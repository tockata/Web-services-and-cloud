namespace BookShop.Services.Controllers
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Http;
    using System.Web.OData;

    using BookShop.Data;
    using BookShop.Services.Models;

    using BookShopSystem.Models;

    [RoutePrefix("api/categories")]
    public class CategoriesController : ApiController
    {
        private BookShopEntities context;

        public CategoriesController()
        {
            this.context = new BookShopEntities();
        }

        [HttpGet]
        [EnableQuery]
        public IHttpActionResult GetCategories()
        {
            var categories = this.context.Categories
                .Select(c => new CategoriesViewModel
                {
                    Id = c.Id.ToString(),
                    Name = c.Name
                });

            return this.Ok(categories);
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetCategoryById(string id)
        {
            Guid categoryId = new Guid(id);
            var category = this.context.Categories
                .Where(c => c.Id == categoryId)
                .Select(c => new CategoriesViewModel
                {
                    Id = c.Id.ToString(),
                    Name = c.Name
                })
                .FirstOrDefault();

            if (category == null)
            {
                return this.NotFound();
            }

            return this.Ok(category);
        }

        [HttpPost]
        public IHttpActionResult PostCategory(AddOrChangeCategoryModelBinder model)
        {
            var exisitngCategory = this.context.Categories
                .FirstOrDefault(c => c.Name == model.Name);

            if (exisitngCategory != null)
            {
                return this.BadRequest("Duplicate category name!");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var newCategory = new Category { Name = model.Name };
            this.context.Categories.Add(newCategory);
            this.context.SaveChanges();

            return this.Ok(newCategory);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditCategory([FromUri]string id, [FromBody]AddOrChangeCategoryModelBinder model)
        {
            Guid categoryId = new Guid(id);
            var exisitngCategory = this.context.Categories
                .FirstOrDefault(c => c.Id == categoryId);

            if (exisitngCategory == null)
            {
                return this.BadRequest("No such category!");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var duplicateCategory = this.context.Categories
                .FirstOrDefault(c => c.Name == model.Name);

            if (duplicateCategory != null)
            {
                return this.BadRequest("Duplicate category name!");
            }

            exisitngCategory.Name = model.Name;
            this.context.Categories.AddOrUpdate(exisitngCategory);
            this.context.SaveChanges();

            return this.Ok(exisitngCategory);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteCategory(string id)
        {
            Guid categoryId = new Guid(id);
            var exisitngCategory = this.context.Categories
                .FirstOrDefault(c => c.Id == categoryId);

            if (exisitngCategory == null)
            {
                return this.BadRequest("No such category!");
            }

            this.context.Categories.Remove(exisitngCategory);
            this.context.SaveChanges();

            return this.Ok(exisitngCategory);
        }
    }
}
