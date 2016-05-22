using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using ClientGraph.Domain;
using ClientGraph.Models;
using ClientGraph.Services;
using ClientGraph.Services.Interfaces;

namespace ClientGraph.Controllers
{
    public abstract class EntityControllerBase<TModel, TEntity, TService> : Controller
        where TModel : ModelBase
        where TEntity : EntityBase
        where TService : IEntityService<TEntity>
    {
        private readonly TService _entityService;
        private readonly RelationshipService _relationshipService;

        protected EntityControllerBase()
        {
            _entityService = Activator.CreateInstance<TService>();
            _relationshipService = new RelationshipService();
        }

        public async Task<ActionResult> Index()
        {
            IList<TEntity> entities = await _entityService.GetAllAsync().ConfigureAwait(false);

            IList<TModel> entityModels = Mapper.Map<IList<TModel>>(entities);

            return View(entityModels);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(TModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();

                TEntity entity = Mapper.Map<TEntity>(model);

                await _entityService.SaveAsync(entity).ConfigureAwait(false);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ViewResult> Edit(Guid id)
        {
            TModel entityModel = await GetModelAsync(id).ConfigureAwait(false);

            return View(entityModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(TModel model)
        {
            if (ModelState.IsValid)
            {
                TEntity entity = Mapper.Map<TEntity>(model);

                await _entityService.SaveAsync(entity).ConfigureAwait(false);

                return RedirectToAction("Details", new { id = model.Id });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ViewResult> Details(Guid id)
        {
            TModel entityModel = await GetModelAsync(id).ConfigureAwait(false);

            if (entityModel != null)
            {
                var relationships = await _relationshipService.GetRelationshipsAsync(id).ConfigureAwait(false);

                entityModel.Relationships = Mapper.Map<IList<RelationshipModel>>(relationships);
            }

            return View(entityModel);
        }

        [HttpGet]
        public async Task<ViewResult> Delete(Guid id)
        {
            TModel model = await GetModelAsync(id).ConfigureAwait(false);

            return View(model);
        }

        [HttpPost]
        public async Task<RedirectToRouteResult> Delete(TEntity entity)
        {
            await _entityService.DeleteAsync(entity.Id).ConfigureAwait(false);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> RestoreVersion(Guid id, string versionId)
        {
            await _entityService.RestoreVersionAsync(id, versionId).ConfigureAwait(false);

            return RedirectToAction("Details", new { id });
        }

        private async Task<TModel> GetModelAsync(Guid id)
        {
            TEntity entity = await _entityService.GetByIdAsync(id).ConfigureAwait(false);

            return Mapper.Map<TModel>(entity);
        }
    }
}