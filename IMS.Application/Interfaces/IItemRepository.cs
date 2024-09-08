using IMS.Application.DTO;
using IMS.Core.Model;

namespace IMS.Application.Interfaces
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
