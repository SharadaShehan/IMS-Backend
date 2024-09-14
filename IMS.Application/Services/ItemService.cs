using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Core.Model;

namespace IMS.Application.Services
{
    public class ItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IEquipmentRepository _equipmentRepository;

        public ItemService(IItemRepository itemRepository, IEquipmentRepository equipmentRepository)
        {
            _itemRepository = itemRepository;
            _equipmentRepository = equipmentRepository;
        }

        public ItemDetailedDTO? GetItemById(int id)
        {
            return _itemRepository.GetItemDTOById(id);
        }

        public List<ItemDTO> GetAllItems(int equipmentId)
        {
            return _itemRepository.GetAllItemDTOs(equipmentId);
        }

        public ResponseDTO<ItemDetailedDTO> CreateNewItem(CreateItemDTO createItemDTO)
        {
            // Check if Equipment Exists
            Equipment? equipment = _equipmentRepository.GetEquipmentEntityById(
                createItemDTO.equipmentId
            );
            if (equipment == null)
                return new ResponseDTO<ItemDetailedDTO>("Equipment Not Found");
            // Prevent Duplicate Item Creation
            if (
                _itemRepository.CheckIfItemExists(
                    createItemDTO.equipmentId,
                    createItemDTO.serialNumber
                )
            )
                return new ResponseDTO<ItemDetailedDTO>("Item Already Exists");
            // Create the Item
            ItemDetailedDTO? itemDTO = _itemRepository.CreateNewItem(createItemDTO, equipment);
            if (itemDTO == null)
                return new ResponseDTO<ItemDetailedDTO>("Failed to Create Item");
            return new ResponseDTO<ItemDetailedDTO>(itemDTO);
        }

        public ResponseDTO<ItemDetailedDTO> DeleteItem(int id)
        {
            // Get the Item to be Deleted
            ItemDetailedDTO? itemDTO = _itemRepository.GetItemDTOById(id);
            if (itemDTO == null)
                return new ResponseDTO<ItemDetailedDTO>("Item Not Found");
            // Delete the Item
            if (!_itemRepository.DeleteItem(id))
                return new ResponseDTO<ItemDetailedDTO>("Failed to Delete Item");
            return new ResponseDTO<ItemDetailedDTO>(itemDTO);
        }
    }
}
