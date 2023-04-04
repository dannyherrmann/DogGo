using DogGo.Models;
using DogGo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DogGo.Controllers
{
    public class WalkerController : Controller
    {
        private readonly IWalkerRepository _walkerRepo;

        public WalkerController(IWalkerRepository walkerRepository)
        {
            _walkerRepo = walkerRepository;
        }

        public ActionResult Index()
        {
            List<Walker> walkers = _walkerRepo.GetAllWalkers();
            return View(walkers);
        }

        public ActionResult Details(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);

            if (walker == null)
            {
                return NotFound();
            }

            return View(walker);
        }
    }
}