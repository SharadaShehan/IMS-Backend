using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Core.Model;

namespace IMS.Application.Services
{
    public class EquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly ILabRepository _labRepository;

        public EquipmentService(
            IEquipmentRepository equipmentRepository,
            ILabRepository labRepository
        )
        {
            _equipmentRepository = equipmentRepository;
            _labRepository = labRepository;
        }

        public EquipmentDetailedDTO? GetEquipmentById(int id)
        {
            return _equipmentRepository.GetEquipmentDTOById(id);
        }

        public List<EquipmentDTO> GetAllEquipments(int labId)
        {
            return _equipmentRepository.GetAllEquipmentDTOs(labId);
        }

        public ResponseDTO<EquipmentDetailedDTO> CreateNewEquipment(
            CreateEquipmentDTO createEquipmentDTO
        )
        {
            // Check if Lab Exists
            Lab? lab = _labRepository.GetLabEntityById(createEquipmentDTO.labId);
            if (lab == null)
                return new ResponseDTO<EquipmentDetailedDTO>("Lab Not Found");
            // Prevent Duplicate Equipment Creation
            if (
                _equipmentRepository.CheckIfEquipmentExists(
                    createEquipmentDTO.name,
                    createEquipmentDTO.model,
                    createEquipmentDTO.labId
                )
            )
                return new ResponseDTO<EquipmentDetailedDTO>("Equipment Already Exists");
            // Create the Equipment
            EquipmentDetailedDTO? equipmentDTO = _equipmentRepository.CreateNewEquipment(
                createEquipmentDTO,
                lab
            );
            if (equipmentDTO == null)
                return new ResponseDTO<EquipmentDetailedDTO>("Failed to Create Equipment");
            return new ResponseDTO<EquipmentDetailedDTO>(equipmentDTO);
        }

        public ResponseDTO<EquipmentDetailedDTO> UpdateEquipment(
            int id,
            UpdateEquipmentDTO updateEquipmentDTO
        )
        {
            // Get the Equipment to be Updated
            Equipment? equipment = _equipmentRepository.GetEquipmentEntityById(id);
            if (equipment == null)
                return new ResponseDTO<EquipmentDetailedDTO>("Equipment Not Found");
            // Update the Equipment
            EquipmentDetailedDTO? updatedEquipmentDTO = _equipmentRepository.UpdateEquipment(
                equipment,
                updateEquipmentDTO
            );
            if (updatedEquipmentDTO == null)
                return new ResponseDTO<EquipmentDetailedDTO>("Failed to Update Equipment");
            return new ResponseDTO<EquipmentDetailedDTO>(updatedEquipmentDTO);
        }

        public ResponseDTO<EquipmentDetailedDTO> DeleteEquipment(int id)
        {
            // Get the Equipment to be Deleted
            EquipmentDetailedDTO? equipmentDTO = _equipmentRepository.GetEquipmentDTOById(id);
            if (equipmentDTO == null)
                return new ResponseDTO<EquipmentDetailedDTO>("Equipment Not Found");
            // Delete the Equipment
            if (!_equipmentRepository.DeleteEquipment(id))
                return new ResponseDTO<EquipmentDetailedDTO>("Failed to Delete Equipment");
            return new ResponseDTO<EquipmentDetailedDTO>(equipmentDTO);
        }
    }
}
