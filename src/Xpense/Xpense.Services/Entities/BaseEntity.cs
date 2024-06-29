﻿namespace Xpense.Services.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool IsDeleted { get; set; }
        
        public void MarkAsDeleted()
        {
            this.IsDeleted = true;
        }

        public void Touch()
        {
            this.LastUpdated = DateTime.Now;
        }
    }
}