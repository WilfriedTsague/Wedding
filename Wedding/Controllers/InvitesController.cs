using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using Wedding.ContexteDB;
using Wedding.Models;

namespace Wedding.Controllers
{
    public class InvitesController : Controller
    {
        private readonly WeddingContext _context;

        public InvitesController(WeddingContext context)
        {
            _context = context;
        }
        // GET: Invites
        public async Task<ActionResult> Index(string sortOrder)
        {
            var Invites = _context.Invites.Include(i => i.Table).Include(i => i.Inviteur);
            return View(await Invites.ToListAsync());
        }

        // GET: Invites/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: InvitesController/Create
        public ActionResult Create()
        {
            ViewData["IdTable"] = new SelectList(_context.Tables, "Id", "NomTable");
            ViewData["IdInviteur"] = new SelectList(_context.Invites, "Id", "NomInvite");
            return View();
        }

        // POST: InvitesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomInvite","PrenomInvite","IdTable", "IdInviteur", "TypeBillets")] Invite invites)
        {
            
            if (ModelState.IsValid)
            {
                

                _context.Add(invites);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdTable"] = new SelectList(_context.Tables, "TablesID", "NomTable");
            ViewData["IdInviteur"] = new SelectList(_context.Invites, "InvitesID", "NomInvite");
            return View(invites);
        }

        // GET: InvitesController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites.FindAsync(id);
            if(invite == null)
            {  return NotFound(); }

            ViewData["IdTable"] = new SelectList(_context.Tables, "TablesID", "NomTable");
            ViewData["IdInviteur"] = new SelectList(_context.Invites, "InvitesID", "NomInvite");

            return View(invite);
        }

        // POST: InvitesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NomInvite", "PrenomInvite", "IdTable", "IdInviteur")] Invite invite)
        {
            if(id != invite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InviteExists(invite.Id))
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
            ViewData["IdTable"] = new SelectList(_context.Tables, "TablesID", "NomTable");
            ViewData["IdInviteur"] = new SelectList(_context.Invites, "InvitesID", "NomInvite");
            return View(invite) ;

        }

        // GET: InvitesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if(id == null)
            {  return NotFound(); }

            var invite = await _context.Invites.Include(i => i.Table)
                .Include(id => id.Inviteur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(invite == null)
                { return NotFound(); }
             return View(invite);
        }

        // POST: InvitesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id )
        {
          var invite = await _context.Invites.FindAsync(id);
            _context.Invites.Remove(invite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmerPresence(int inviteId)
        {
            var invite = await _context.Invites.Include(i => i.Table).FirstOrDefaultAsync(i => i.Id == inviteId);

            if (invite == null)
            { return NotFound(); }

            var table = invite.Table;

            var nombreInvitesConfirmes = _context.Invites.Count(i => i.IdTable == table.Id );
            table.StatutDuJour = nombreInvitesConfirmes == 0 ? Statut.En_cours:nombreInvitesConfirmes == table.NbrePlaces ? Statut.Pleine: table.StatutDuJour;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InviteExists(int inviteID)
        {
            return _context.Invites.Any(e => e.Id == inviteID);
        }
    }
}
