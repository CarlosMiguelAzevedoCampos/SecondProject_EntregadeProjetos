namespace CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.Interface
{
    public interface ICamundaService
    {
        bool StartWorkFlow(Core.Model.DeliveryFileSystem delivery);
        void RegistWorkers();
    }
}
