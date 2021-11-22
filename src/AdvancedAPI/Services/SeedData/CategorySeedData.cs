using Entities;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SeedData
{
    public class CategorySeedData : ISeedData
    {
        private readonly IRepository<Category> _repository;

        public CategorySeedData(IRepository<Category> repository)
        {
            this._repository = repository;
        }
        public void InitializeData()
        {
            Console.WriteLine(nameof(CategorySeedData));

            string name = "programing language";
            if (this._repository.TableNoTracking.Any(c => c.Name == name))
            {
                this._repository.Add(new Category()
                {
                    Name = name
                });
            }
        }
    }
}
