using DogGo.Models;
using DogGo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DogGo.Controllers
{
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepo;

        public OwnerController(IOwnerRepository ownerRepository)
        {
            _ownerRepo = ownerRepository;
        }

        public ActionResult Index()
        {
            List<Owner> owners = _ownerRepo.GetAllOwners();
            return View(owners);
        }

        // details does not work if owner has no dogs yet
        public ActionResult Details(int id)
        {
            Owner owner = _ownerRepo.GetOwnerById(id);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Owner owner)
        {
            try
            {
                _ownerRepo.AddOwner(owner);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(owner);
            }
        }

        // Delete does not work if owner does not have any dogs yet
        public ActionResult Delete(int id)
        {
            Owner owner = _ownerRepo.GetOwnerById(id);

            return View(owner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Owner owner)
        {
            try
            {
                _ownerRepo.DeleteOwner(owner.Id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(owner);
            }
        }

        // Edit does not work if owner does not have any dogs yet
        public ActionResult Edit(int id)
        {
            Owner owner = _ownerRepo.GetOwnerById(id);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Owner owner)
        {
            try 
            {
                _ownerRepo.UpdateOwner(owner);

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View(owner);
            }
        }
    }
}