﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    public class User : IdentityUser<int>, IEntity
    {
        public User()
        {
            this.IsActive = true;
        }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Range(1, 150)]
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public bool IsActive { get; set; }

        public DateTimeOffset LoginDate { get; set; }
        public DateTimeOffset LastActivityDate { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // UserName
            builder.Property(p => p.UserName)
                .IsRequired()
                .HasMaxLength(50);

            // PasswordHash
            builder.Property(p => p.PasswordHash)
                .IsRequired();
        }
    }

    public enum Gender
    {
        [Display(Name = "هیچکدام")]
        None = 0,

        [Display(Name = "زن")]
        Female = 1,

        [Display(Name = "مرد")]
        Male = 2,
    }
}
