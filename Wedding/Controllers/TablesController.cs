using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wedding.ContexteDB;
using Wedding.Models;

namespace Wedding.Controllers
{
    public class TablesController : Controller
    {
        private readonly WeddingContext _context;

        public TablesController(WeddingContext context)
        {
            _context = context;
        }
        // GET: TablesController
        public async Task<ActionResult> Index(string sortOrder)
        {
            var tables = from t in _context.Tables select t;

            switch (sortOrder)
            {
                case "NomTable":
                    tables = tables.OrderBy(t => t.NomTable);
                    break;
                case "NbrePlaces":
                    tables = tables.OrderBy(t => t.NbrePlaces);
                    break;
                case "Statut":
                    tables = tables.OrderBy(t => t.Statut);
                    break;
                default:
                    tables = tables.OrderBy(t => t.NomTable);
                    break;
            }
            return View(await tables.ToListAsync());
        }

        // GET: TablesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var table = await _context.Tables.Include(t => t.invites)
                                             .FirstOrDefaultAsync(t => t.Id == id);
            if (table == null)
            {
                return NotFound();
            }
            return View(table);
        }

        // GET: TablesController/Create
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: TablesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomTable","NbrePlaces")] Table table)
        {
            
            if(ModelState.IsValid)
            {
                table.Statut = Statut.Vide;
                table.StatutDuJour = Statut.Vide;
                table.NbreInvitePresent = 0;

                _context.Add(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
         
                return View(table);
            
        }

        // GET: TablesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            var table = await _context.Tables.FindAsync(id);
            if(table == null)
            {

            return NotFound(); 
            }
            return View(table);
        }

        // POST: TablesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NomTable", "NbrePlaces", "Statut", "StatutDuJour")] Table table)
        {
          if(id != table.Id)
            {
                return NotFound();
            }

          if(ModelState.IsValid)
            {
                try
                {
                    _context.Update(table);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if(!TableExistes(table.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
          return View(table) ;
        }

        // GET: Tables/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // POST: Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool TableExistes(int tableID)

        {
            return _context.Tables.Any(e => e.Id == tableID);
        }
    }
}
