using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = string.Empty;
        public Guid OwnerUserId { get; private set; }


        private Project() { } // EF CORE

        public Project(string name, Guid ownerUserId)
        {
            Name = name;
            OwnerUserId = ownerUserId;
        }
    }
}