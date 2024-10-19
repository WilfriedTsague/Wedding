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
        public async Task<IActionResult> Index(string sortOrder, string searchNomTable, string searchStatut)
        {
            ViewBag.NomTableSortParm = String.IsNullOrEmpty(sortOrder) ? "nomtable_desc" : "";
            ViewBag.NbrePlacesSortParm = sortOrder == "NbrePlaces" ? "nbreplace_desc" : "NbrePlaces";
            ViewBag.NombreInvitesSortParm = sortOrder == "NombreInvites" ? "nombreinvites_desc" : "NombreInvites";
            ViewBag.StatutSortParm = sortOrder == "Statut" ? "statut_desc" : "Statut";
            ViewBag.StatutDuJourSortParm = sortOrder == "StatutDuJour" ? "statutdujour_desc" : "StatutDuJour";

            var tables = from t in _context.Tables
                         select t;

            if (!string.IsNullOrEmpty(searchNomTable))
            {
                tables = tables.Where(t => t.NomTable.Contains(searchNomTable));
            }

            if (!string.IsNullOrEmpty(searchStatut) && Enum.TryParse(searchStatut, out Statut statutEnum))
            {
                tables = tables.Where(t => t.Statut == statutEnum);
            }

            switch (sortOrder)
            {
                case "nomtable_desc":
                    tables = tables.OrderByDescending(t => t.NomTable);
                    break;
                case "NbrePlaces":
                    tables = tables.OrderBy(t => t.NbrePlaces);
                    break;
                case "nbreplace_desc":
                    tables = tables.OrderByDescending(t => t.NbrePlaces);
                    break;
                case "NombreInvites":
                    tables = tables.OrderBy(t => t.NombreInvites);
                    break;
                case "nombreinvites_desc":
                    tables = tables.OrderByDescending(t => t.NombreInvites);
                    break;
                case "Statut":
                    tables = tables.OrderBy(t => t.Statut);
                    break;
                case "statut_desc":
                    tables = tables.OrderByDescending(t => t.Statut);
                    break;
                case "StatutDuJour":
                    tables = tables.OrderBy(t => t.StatutDuJour);
                    break;
                case "statutdujour_desc":
                    tables = tables.OrderByDescending(t => t.StatutDuJour);
                    break;
                default:
                    tables = tables.OrderBy(t => t.NomTable);
                    break;
            }

            ViewData["searchNomTable"] = searchNomTable;
            ViewData["searchStatut"] = searchStatut;

            return View(await tables.ToListAsync());
        }


        // GET: TablesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var table = await _context.Tables.Include(t => t.Invites)
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
                // Vérifier si le nom de la table existe déjà         
                var existingTable = await _context.Tables.FirstOrDefaultAsync(t => t.NomTable == table.NomTable);
                if (existingTable != null)
                {
                    ModelState.AddModelError("NomTable", "Ce nom de table existe déjà.");
                    return View(table);
                }

                table.Statut = Statut.Vide;
                table.StatutDuJour = Statut.Vide;
                table.NbreInvitePresent = 0;
                table.NombreInvites = 0;

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
        public async Task<IActionResult> Edit(int id, [Bind("NomTable", "NbrePlaces")] Table table)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var tableToUpdate = await _context.Tables.Include(t => t.Invites).FirstOrDefaultAsync(t => t.Id == id);
                    if (tableToUpdate != null)
                    {
                        if (table.NbrePlaces < tableToUpdate.Invites.Count)
                        {
                            ModelState.AddModelError("", "Le nombre de places ne peut pas être inférieur au nombre d'invités déjà enregistrés.");
                            return View(table);
                        }

                        tableToUpdate.NomTable = table.NomTable;
                        tableToUpdate.NbrePlaces = table.NbrePlaces;

                        if (tableToUpdate.NbrePlaces > tableToUpdate.Invites.Count && table.NombreInvites>0)
                        {

                            tableToUpdate.Statut = Statut.En_cours;
                        }

                        _context.Update(tableToUpdate);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableExistes(table.Id))
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
            return View(table);
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
