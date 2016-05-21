using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using ClientGraph.Domain;
using ClientGraph.Models;
using ClientGraph.Services.Interfaces;

namespace ClientGraph.Controllers
{
    public abstract class EntityControllerBase<TModel, TEntity, TService> : Controller
        where TModel : ModelBase
        where TEntity : EntityBase
        where TService : IEntityService<TEntity>
    {
        private readonly TService _entityService;

        protected EntityControllerBase()
        {
            _entityService = Activator.CreateInstance<TService>();
        }

        public ActionResult Index()
        {
            IList<TEntity> entities = _entityService.GetAll();

            IList<TModel> entityModels = Mapper.Map<IList<TModel>>(entities);

            return View(entityModels);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();

                TEntity entity = Mapper.Map<TEntity>(model);

                _entityService.Save(entity);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            TModel entityModel = GetModel(id);

            return View(entityModel);
        }

        [HttpPost]
        public ActionResult Edit(TModel model)
        {
            if (ModelState.IsValid)
            {
                TEntity entity = Mapper.Map<TEntity>(model);

                _entityService.Save(entity);

                return RedirectToAction("Details", new { id = model.Id });
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Details(Guid id)
        {
            TModel entityModel = GetModel(id);

            return View(entityModel);
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            TModel model = GetModel(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(TEntity entity)
        {
            _entityService.Delete(entity.Id);

            return RedirectToAction("Index");
        }

        private TModel GetModel(Guid id)
        {
            TEntity entity = _entityService.GetById(id);

            return Mapper.Map<TModel>(entity);
        }
    }
}