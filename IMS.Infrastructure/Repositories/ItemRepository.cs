using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Core.Model;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly DataBaseContext _dbContext;

        public ItemRepository(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Item? GetItemEntityById(int id)
        {
            return _dbContext.items.Where(i => i.ItemId == id && i.IsActive).FirstOrDefault();
        }

        public ItemDetailedDTO? GetItemDTOById(int id)
        {
            Maintenance? lastMaintenance = _dbContext
                .maintenances.Where(m => m.ItemId == id && m.Status != "Canceled" && m.IsActive)
                .OrderByDescending(m => m.EndDate)
                .FirstOrDefault();
            return _dbContext
                .items.Where(i => i.ItemId == id && i.IsActive)
                .Select(i => new ItemDetailedDTO
                {
                    itemId = i.ItemId,
                    itemName = i.Equipment.Name,
                    itemModel = i.Equipment.Model,
                    imageUrl = i.Equipment.ImageURL,
                    equipmentId = i.EquipmentId,
                    labId = i.Equipment.LabId,
                    labName = i.Equipment.Lab.LabName,
                    serialNumber = i.SerialNumber,
                    lastMaintenanceOn = lastMaintenance != null ? lastMaintenance.EndDate : null,
                    lastMaintenanceBy =
                        lastMaintenance != null
                            ? lastMaintenance.Technician.FirstName
                                + " "
                                + lastMaintenance.Technician.LastName
                            : null,
                    status = i.Status,
                })
                .FirstOrDefault();
        }

        public List<ItemDTO> GetAllItemDTOs(int equipmentId)
        {
            return _dbContext
                .items.Where(i => i.IsActive && i.EquipmentId == equipmentId)
                .Select(i => new ItemDTO
                {
                    itemId = i.ItemId,
                    imageUrl = i.Equipment.ImageURL,
                    equipmentId = i.EquipmentId,
                    serialNumber = i.SerialNumber,
                    status = i.Status,
                })
                .ToList();
        }

        public bool CheckIfItemExists(int equipmentId, string serialNumber)
        {
            Item? existingItem = _dbContext
                .items.Where(i =>
                    i.EquipmentId == equipmentId && i.SerialNumber == serialNumber && i.IsActive
                )
                .FirstOrDefault();
            return existingItem != null;
        }

        public ItemDetailedDTO? CreateNewItem(CreateItemDTO createItemDTO, Equipment equipment)
        {
            Item item = new Item
            {
                EquipmentId = createItemDTO.equipmentId,
                Equipment = equipment,
                SerialNumber = createItemDTO.serialNumber,
                Status = "Available",
                IsActive = true,
            };
            _dbContext.items.Add(item);
            _dbContext.SaveChanges();
            return GetItemDTOById(item.ItemId);
        }

        public bool DeleteItem(int id)
        {
            // delete item
            _dbContext
                .items.Where(i => i.ItemId == id)
                .ExecuteUpdate(i => i.SetProperty(i => i.IsActive, false));
            // delete item reservations of the item
            _dbContext
                .itemReservations.Where(ir => ir.ItemId == id)
                .ExecuteUpdate(ir => ir.SetProperty(ir => ir.IsActive, false));
            // delete maintenance logs of the item
            _dbContext
                .maintenances.Where(m => m.ItemId == id)
                .ExecuteUpdate(m => m.SetProperty(m => m.IsActive, false));
            _dbContext.SaveChanges();
            return true;
        }
    }
}
