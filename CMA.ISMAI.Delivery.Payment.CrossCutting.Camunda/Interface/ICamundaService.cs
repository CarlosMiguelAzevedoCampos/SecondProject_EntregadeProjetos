namespace CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda.Interface
{
    public interface ICamundaService
    {
        bool StartWorkFlow(Core.Model.DeliveryFileSystem delivery);
        void RegistWorkers();
    }
}
