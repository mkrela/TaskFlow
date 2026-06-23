using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Application.Services.Tasks;
using TaskFlow.Application.Validators.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using Xunit;
using DomainTaskStatus = TaskFlow.Domain.Enums.TaskStatus;

namespace TaskFlow.Tests.Services;

/// <summary>
/// Tests unitaires de <see cref="TaskService"/>.
/// Objectif : vérifier la logique métier du service sans base de données réelle.
/// </summary>
public class TaskServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Task_And_Save()
    {
        // Arrange
        // 1) On prépare un faux repository en mémoire (pas de SQL Server).
        var repository = new FakeTaskRepository();

        // 2) On construit le service à tester avec ses dépendances.
        var service = CreateService(repository);

        // 3) On prépare la requête d'entrée (DTO) simulant une requête API valide.
        var request = new CreateTaskRequest
        {
            Title = "Nouvelle tâche",
            ProjectId = Guid.NewGuid(),
            CreatedByUserId = Guid.NewGuid(),
            Description = "Description test"
        };

        // Act
        // Exécution de la méthode métier à tester.
        var result = await service.CreateAsync(request);

        // Assert
        // Vérifie le résultat fonctionnel + les effets de bord (persistance + SaveChanges).
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Nouvelle tâche", result.Title);
        Assert.Equal(1, repository.Items.Count);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Null_When_Task_Not_Found()
    {
        // Arrange
        // Repository vide -> aucune tâche trouvable.
        var repository = new FakeTaskRepository();
        var service = CreateService(repository);

        var request = new UpdateTaskRequest
        {
            Title = "Titre modifié",
            Description = "Desc modifiée",
            Priority = TaskPriority.High
        };

        // Act
        // On tente de modifier une tâche qui n'existe pas.
        var result = await service.UpdateAsync(Guid.NewGuid(), request);

        // Assert
        // Le service doit renvoyer null et ne pas appeler SaveChanges.
        Assert.Null(result);
        Assert.Equal(0, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Task_When_Found()
    {
        // Arrange
        // 1) On prépare une tâche existante.
        var repository = new FakeTaskRepository();
        var existing = new TaskItem("Titre initial", Guid.NewGuid(), Guid.NewGuid());
        await repository.AddAsync(existing);

        // 2) On crée le service.
        var service = CreateService(repository);

        // 3) Données de mise à jour.
        var request = new UpdateTaskRequest
        {
            Title = "Titre modifié",
            Description = "Desc modifiée",
            Priority = TaskPriority.High,
            DueDate = DateTime.UtcNow.AddDays(3)
        };

        // Act
        var result = await service.UpdateAsync(existing.Id, request);

        // Assert
        // Vérifie que les champs sont modifiés et que SaveChanges est bien appelé.
        Assert.NotNull(result);
        Assert.Equal("Titre modifié", result.Title);
        Assert.Equal(TaskPriority.High, result.Priority);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task ChangeStatusAsync_Should_Return_True_And_Save_When_Found()
    {
        // Arrange
        var repository = new FakeTaskRepository();
        var existing = new TaskItem("Task", Guid.NewGuid(), Guid.NewGuid());
        await repository.AddAsync(existing);

        var service = CreateService(repository);

        // Act
        // On change le statut de la tâche.
        var updated = await service.ChangeStatusAsync(existing.Id, DomainTaskStatus.Done);

        // Assert
        // Le service confirme le changement et persiste la modification.
        Assert.True(updated);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    /// <summary>
    /// Fabrique une instance du service avec un faux repository et les vrais validators.
    /// </summary>
    private static TaskService CreateService(FakeTaskRepository repository)
    {
        return new TaskService(
            repository,
            new CreateTaskRequestValidator(),
            new UpdateTaskRequestValidator());
    }

    /// <summary>
    /// Fake repository en mémoire.
    /// Il remplace la base de données pour garder des tests rapides, isolés et déterministes.
    /// </summary>
    private sealed class FakeTaskRepository : ITaskRepository
    {
        /// <summary>
        /// "Table" en mémoire qui stocke les tâches.
        /// </summary>
        public List<TaskItem> Items { get; } = [];

        /// <summary>
        /// Compteur pour vérifier si SaveChanges est appelé.
        /// </summary>
        public int SaveChangesCallCount { get; private set; }

        public Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
        {
            // Simule l'ajout d'une entité dans une base de données (équivalent d'un INSERT).
            // Ici on n'utilise pas SQL : on stocke l'objet dans une liste mémoire.
            Items.Add(taskItem);

            // La méthode est asynchrone (retourne Task), mais il n'y a aucun vrai travail async
            // (pas de réseau/disque). On retourne donc une tâche déjà terminée.
            return Task.CompletedTask;
        }

        public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Simule une recherche par clé primaire (WHERE Id = ...).
            // FirstOrDefault retourne null si aucun élément n'est trouvé.
            TaskItem? item = Items.FirstOrDefault(x => x.Id == id);

            // On encapsule le résultat dans une Task pour respecter la signature async.
            return Task.FromResult(item);
        }

        public Task<IReadOnlyList<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            // Simule une requête filtrée (WHERE ProjectId = ...).
            // On récupère toutes les tâches liées au projet demandé.
            IReadOnlyList<TaskItem> list = Items
                .Where(x => x.ProjectId == projectId)
                .ToList();

            // Retour asynchrone simulé.
            return Task.FromResult(list);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Simule l'appel à SaveChangesAsync() d'EF Core.
            // Dans ce fake, il n'y a rien à persister réellement, donc on incrémente un compteur
            // pour vérifier dans les assertions que la sauvegarde a bien été demandée.
            SaveChangesCallCount++;

            // Fin immédiate : pas de vraie opération I/O.
            return Task.CompletedTask;
        }
    }
}