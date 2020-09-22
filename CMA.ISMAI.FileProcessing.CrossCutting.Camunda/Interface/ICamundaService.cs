namespace CMA.ISMAI.Delivery.FileProcessing.CrossCutting.Camunda.Interface
{
    public interface ICamundaService
    {
        bool StartWorkFlow(Core.Model.Delivery delivery);
        void RegistWorkers();
    }
}
