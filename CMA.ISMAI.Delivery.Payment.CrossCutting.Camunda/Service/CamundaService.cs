
using CamundaClient;
using CamundaClient.Dto;
using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda.Interface;
using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using CMA.ISMAI.Delivery.Payment.Domain.Model;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda.Service
{
    public class CamundaService : ICamundaService
    {
        private readonly CamundaEngineClient camundaEngineClient;
        private readonly string filePath;
        private Timer pollingTimer;
        private IDictionary<string, Action<ExternalTask>> workers;
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;
        private readonly IQueueService _queueService;
        private readonly ILoggingService _log;
        private readonly IConfiguration _config;

        public CamundaService(IMediator mediator, INotificationService notificationService, IQueueService queueService, ILoggingService log)
        {
            _config = new ConfigurationBuilder()
                                                      .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                                                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                      .AddEnvironmentVariables()
                                                      .Build(); camundaEngineClient = new CamundaEngineClient(new System.Uri(_config.GetSection("Camunda:Uri").Value), null, null);
            filePath = $"CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda.WorkFlow.StudentPaymentISMAI.bpmn";
            workers = new Dictionary<string, Action<ExternalTask>>();
            _mediator = mediator;
            _notificationService = notificationService;
            _queueService = queueService;
            _log = log;
        }

        public void RegistWorkers()
        {
            workers = new Dictionary<string, Action<ExternalTask>>();
            registerWorker("notify_for_payment", externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getStudentEmail = returnVariableValue(delivery, "studentEmail");
                    bool result = _notificationService.SendEmail(getStudentEmail.Value.ToString(), "Olá! <br/> <br/> Os teus ficheiros foram aprovados. <br/> <br/>Agora, podes pagar a tua entrega! <br/> <br/> Obrigado, <br/> <br/> Delivery System.");

                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable.Add("ok", result);
                    dictionaryToPassVariable.Add("Worker", "notify_for_payment");
                    camundaEngineClient.ExternalTaskService.Complete("StudentPaymentISMAI", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

            registerWorker("verify_payment", async externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getCourseName = returnVariableValue(delivery, "courseName");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");
                    var getCordenatorName = returnVariableValue(delivery, "cordenatorName");
                    var deliveryPayment = new VerifyPaymentOfDeliveryCommand(getStudentNumber.Value.ToString(), getInstituteName.Value.ToString(), getCourseName.Value.ToString(), _config.GetSection("FilePathPayment:Path").Value);

                    var result = await _mediator.Send(deliveryPayment);

                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    if (!dictionaryToPassVariable.ContainsKey("paid"))
                    {
                        dictionaryToPassVariable.Add("paid", result.IsValid);
                    }
                    else
                    {
                        dictionaryToPassVariable["paid"] = result.IsValid;
                    }
                    _log.Info($"Payment has been done? {result.IsValid}");
                    dictionaryToPassVariable["Worker"] = "verify_payment";
                    camundaEngineClient.ExternalTaskService.Complete("StudentPaymentISMAI", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

            registerWorker("notify_university", externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getStudentName = returnVariableValue(delivery, "studentName");
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getCourseName = returnVariableValue(delivery, "courseName");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");
                    var getCordenatorName = returnVariableValue(delivery, "cordenatorName");

                    bool result = _notificationService.SendEmail(_config.GetSection("Notification:University").Value, string.Format(@"Olá, <br/> A entrega de {0}, foi paga! <br/> <br/> 
                            Nome da Instituição: {1}, Número do Aluno:{2}, Curso: {3}. <br/><br/> Obrigado, <br/> <br/> Delivery System", getStudentName.Value.ToString(),
                        getInstituteName.Value.ToString(), getStudentNumber.Value.ToString(), getCourseName.Value.ToString()));

                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable["ok"] = result;
                    dictionaryToPassVariable["Worker"] = "notify_university";
                    camundaEngineClient.ExternalTaskService.Complete("StudentPaymentISMAI", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });


            registerWorker("send_to_broker", externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getStudentName = returnVariableValue(delivery, "studentName");
                    var getDeliveryUrl = returnVariableValue(delivery, "fileUrl");
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getStudentEmail = returnVariableValue(delivery, "studentEmail");
                    var getCourseName = returnVariableValue(delivery, "courseName");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");
                    var getDeliveryTime = returnVariableValue(delivery, "deliveryTime");
                    var getCordenatorName = returnVariableValue(delivery, "cordenatorName");
                    var getTitle = returnVariableValue(delivery, "title");
                    var getDefenition = returnVariableValue(delivery, "defenitionOfDelivery");
                    var getpublicdefenition = returnVariableValue(delivery, "publicPDFVersionName");
                    var getprivatedefenition = returnVariableValue(delivery, "privatePDFVersionName");
                    var getPath = returnVariableValue(delivery, "deliveryPath");
                    var getId = returnVariableValue(delivery, "id");


                    Core.Model.DeliveryFileSystem deliveryFileSystem = new Core.Model.DeliveryFileSystem(Guid.Parse(getId.Value.ToString())
                        , getStudentName.Value.ToString(), getInstituteName.Value.ToString(), getCourseName.Value.ToString(), getStudentEmail.Value.ToString(), getStudentNumber.Value.ToString(), DateTime.Parse(getDeliveryTime.Value.ToString()),
                        getCordenatorName.Value.ToString(), getTitle.Value.ToString(), getDefenition.Value.ToString(), getpublicdefenition.Value.ToString(), getprivatedefenition.Value.ToString(), getPath.Value.ToString());


                    bool validation = _queueService.SendToQueue(deliveryFileSystem, "FileProcessing");
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable["ok"] = validation;
                    dictionaryToPassVariable["Worker"] = "send_to_broker";

                    camundaEngineClient.ExternalTaskService.Complete("StudentPaymentISMAI", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });




            registerWorker("notify_student_payment", externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getStudentName = returnVariableValue(delivery, "studentName");
                    var getDeliveryUrl = returnVariableValue(delivery, "fileUrl");
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getStudentEmail = returnVariableValue(delivery, "studentEmail");
                    var getCourseName = returnVariableValue(delivery, "courseName");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");
                    var getWorker = returnVariableValue(delivery, "Worker");

                    bool result = _notificationService.SendEmail(getStudentEmail.Value.ToString(), "Olá! <br/> <br/> Os teus ficheiros foram aprovados. <br/> <br/>Agora, podes pagar a tua entrega! <br/> <br/> Obrigado, <br/> <br/> Delivery System.");
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable["Worker"] = "notify_student";
                    camundaEngineClient.ExternalTaskService.Complete("StudentPaymentISMAI", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

            registerWorker("manual_processing_payment", externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getStudentName = returnVariableValue(delivery, "studentName");
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getStudentEmail = returnVariableValue(delivery, "studentEmail");
                    var getCourseName = returnVariableValue(delivery, "courseName");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");
                    var getWorker = returnVariableValue(delivery, "worker");

                    _notificationService.SendEmail(_config.GetSection("Notification:ManualProcessing").Value, string.Format("Olá, <br/> Algo correu mal na entrega. <br/> <br/> A entrega falhou no diagrama Payment. <br/> <br/> Nome do Estudante:{0}, Nome da Instituição: {1}, Número de Aluno:{2}, Curso:{3}. <br/> <br/> Ele falhou na fase {4} . <br/> <br/> Obrigado",
                        getStudentName.Value.ToString(), getInstituteName.Value.ToString(), getStudentNumber.Value.ToString(), getCourseName.Value.ToString(), getWorker.Value.ToString()));
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    camundaEngineClient.ExternalTaskService.Complete("StudentPaymentISMAI", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

            pollingTimer = new Timer(_ => StartPolling(), null, Convert.ToInt64(_config.GetSection("TimeToFetch:Time").Value), Timeout.Infinite);

        }

        private Dictionary<string, object> returnDictionary(Dictionary<string, Variable> delivery)
        {
            Dictionary<string, object> valuesforNextIteration = new Dictionary<string, object>();
            foreach (var item in delivery)
                valuesforNextIteration.Add(item.Key, item.Value.Value);
            return valuesforNextIteration;
        }

        public Variable returnVariableValue(Dictionary<string, Variable> externalTaskVariables, string key)
        {
            var delivery = externalTaskVariables;
            var getValue = new Variable();
            delivery.TryGetValue(key, out getValue);
            return getValue;
        }

        private void StartPolling()
        {
            PollTasks();
            pollingTimer.Change(Convert.ToInt64(_config.GetSection("TimeToFetch:Time").Value), Timeout.Infinite);
        }

        private void PollTasks()
        {
            try
            {
                var tasks = camundaEngineClient.ExternalTaskService.FetchAndLockTasks("StudentPaymentISMAI", Convert.ToInt32(_config.GetSection("TaskPerFetch:Tasks").Value), workers.Keys, long.Parse((Convert.ToInt64(_config.GetSection("TimeToFetch:Time").Value) / 2).ToString()), null);
                Parallel.ForEach(
                    tasks,
                    new ParallelOptions { MaxDegreeOfParallelism = 1 },
                    (externalTask) =>
                    {
                        workers[externalTask.TopicName](externalTask);
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _log.Fatal(ex.ToString());
            }
        }

        private void registerWorker(string topicName, Action<ExternalTask> action)
        {
            workers.Add(topicName, action);
        }


        public bool StartWorkFlow(Core.Model.Delivery delivery)
        {

            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters = AddValuesToTheDictionary(parameters, delivery);
                FileParameter file = FileParameter.FromManifestResource(Assembly.GetExecutingAssembly(), filePath);
                string deployId = camundaEngineClient.RepositoryService.Deploy("StudentPaymentISMAI", new List<object> {
                    file });
                if (TheDeployWasDone(deployId))
                {
                    camundaEngineClient.BpmnWorkflowService.StartProcessInstance("StudentPaymentISMAI", parameters);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        private bool TheDeployWasDone(string deployId)
        {
            return deployId != string.Empty;
        }



        private Dictionary<string, object> AddValuesToTheDictionary(Dictionary<string, object> parameters, Core.Model.Delivery message)
        {
            foreach (PropertyInfo propertyInfo in message.GetType().GetProperties())
                parameters.Add(propertyInfo.Name, propertyInfo.GetValue(message, null));
            return parameters;
        }
    }
}
