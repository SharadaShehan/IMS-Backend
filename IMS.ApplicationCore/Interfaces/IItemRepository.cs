using IMS.ApplicationCore.DTO;
using IMS.ApplicationCore.Model;

namespace IMS.ApplicationCore.Interfaces
{
    public interface IItemRepository
    {
        Item? GetItemEntityById(int id);
        ItemDetailedDTO? GetItemDTOById(int id);
        List<ItemDTO> GetAllItemDTOs(int equipmentId);
        bool CheckIfItemExists(int equipmentId, string serialNumber);
        ItemDetailedDTO? CreateNewItem(CreateItemDTO createItemDTO, Equipment equipment);
        bool DeleteItem(int id);
    }
}
