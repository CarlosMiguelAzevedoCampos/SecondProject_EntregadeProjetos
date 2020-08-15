namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.Interface
{
    public interface ICamundaService
    {
        bool StartWorkFlow(Core.Model.Delivery delivery);
        void RegistWorkers();
    }
}
