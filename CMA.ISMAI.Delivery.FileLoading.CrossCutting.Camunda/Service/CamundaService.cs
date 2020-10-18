using CamundaClient;
using CamundaClient.Dto;
using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.Interface;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model.Commands;
using CMA.ISMAI.Delivery.Logging.Interface;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.Service
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
                                                      .Build();
            camundaEngineClient = new CamundaEngineClient(new System.Uri(_config.GetSection("Camunda:Uri").Value), null, null);
            filePath = $"CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.WorkFlow.FileLoadingISMAI.bpmn";
            workers = new Dictionary<string, Action<ExternalTask>>();
            _mediator = mediator;
            _notificationService = notificationService;
            _queueService = queueService;
            _log = log;
        }

        public bool StartWorkFlow(Core.Model.Delivery delivery)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters = AddValuesToTheDictionary(parameters, delivery);
                FileParameter file = FileParameter.FromManifestResource(Assembly.GetExecutingAssembly(), filePath);
                string deployId = camundaEngineClient.RepositoryService.Deploy("FileLoading", new List<object> {
                    file });
                if (TheDeployWasDone(deployId))
                {
                    camundaEngineClient.BpmnWorkflowService.StartProcessInstance("FileLoading", parameters);
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        private Dictionary<string, object> AddValuesToTheDictionary(Dictionary<string, object> parameters, Core.Model.Delivery message)
        {
            foreach (PropertyInfo propertyInfo in message.GetType().GetProperties())
                parameters.Add(propertyInfo.Name, propertyInfo.GetValue(message, null));
            parameters.Add("download", message.GetType() == typeof(Core.Model.DeliveryWithLink));

            return parameters;
        }

        private bool TheDeployWasDone(string deployId)
        {
            return deployId != string.Empty;
        }

        public Variable returnVariableValue(Dictionary<string, Variable> externalTaskVariables, string key)
        {
            var delivery = externalTaskVariables;
            var getValue = new Variable();
            delivery.TryGetValue(key, out getValue);
            return getValue;
        }

        public void RegistWorkers()
        {
            workers = new Dictionary<string, Action<ExternalTask>>();

            registerWorker("download_files", async externalTask =>
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
                    var getId = returnVariableValue(delivery, "id");
                    DownloadFileFromUrlCommand downloadCommand = new DownloadFileFromUrlCommand(Guid.Parse(getId.Value.ToString())
                        , getStudentName.Value.ToString(), getInstituteName.Value.ToString(), getCourseName.Value.ToString(), getStudentEmail.Value.ToString(), getStudentNumber.Value.ToString(), DateTime.Parse(getDeliveryTime.Value.ToString()),
                        getCordenatorName.Value.ToString(), getDefenition.Value.ToString(), getTitle.Value.ToString(), getDeliveryUrl.Value.ToString(), getpublicdefenition.Value.ToString(), getprivatedefenition.Value.ToString());
                    var validation = await _mediator.Send(downloadCommand);
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable.Add("ok", validation.IsValid);
                    dictionaryToPassVariable.Add("Worker", "download_files");
                    camundaEngineClient.ExternalTaskService.Complete("FileLoading", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

            registerWorker("verify_files_names", async externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getId = returnVariableValue(delivery, "id");
                    var getStudentName = returnVariableValue(delivery, "studentName");
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");
                    var getCourseName = returnVariableValue(delivery, "courseName");
                    var getpublicdefenition = returnVariableValue(delivery, "publicPDFVersionName");
                    var getprivatedefenition = returnVariableValue(delivery, "privatePDFVersionName");


                    VerifyFilesNameCommand verifyFilesCommand = new VerifyFilesNameCommand(Guid.Parse(getId.Value.ToString()), string.Format(@"{0}{1}_{2}_{3}_{4}.zip", _config.GetSection("FilePathZip:Path").Value, getStudentNumber.Value.ToString(), getInstituteName.Value.ToString(),
                   getStudentName.Value.ToString(), getCourseName.Value.ToString()),
                  string.Format(@"{0}{1}_{2}_{3}_{4}{5}", _config.GetSection("FilePathUnzip:Path").Value, getStudentNumber.Value.ToString(), getInstituteName.Value.ToString(),
                   getStudentName.Value.ToString(), getCourseName.Value.ToString(), IsRunningLinux() ? "//" : "\\"), getpublicdefenition.Value.ToString(), getprivatedefenition.Value.ToString());


                    var validation = await _mediator.Send(verifyFilesCommand);
                    if (!validation.IsValid)
                    {
                        DeleteFiles(string.Format(@"{0}{1}{2}_{3}_{4}_{5}.zip", _config.GetSection("FilePathUnzip:Path").Value, IsRunningLinux() ? "//" : "\\", getStudentNumber.Value.ToString(), getInstituteName.Value.ToString(),
                            getStudentName.Value.ToString(), getCourseName.Value.ToString()));

                        DeleteFiles(string.Format(@"{0}{1}_{2}_{3}_{4}", _config.GetSection("FilePathUnzip:Path").Value, getStudentNumber.Value.ToString(), getInstituteName.Value.ToString(),
                            getStudentName.Value.ToString(), getCourseName.Value.ToString()));
                    }
                
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable.Add("filesnames", validation.IsValid);
                    dictionaryToPassVariable["Worker"] = "verify_files_names";
                    camundaEngineClient.ExternalTaskService.Complete("FileLoading", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

           
            registerWorker("verify_files", async externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getId = returnVariableValue(delivery, "id");
                    var getStudentName = returnVariableValue(delivery, "studentName");
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");
                    var getCourseName = returnVariableValue(delivery, "courseName");

                    VerifyFilesCommand verifyFilesCommand = new VerifyFilesCommand(Guid.Parse(getId.Value.ToString()),
                            string.Format(@"{0}{1}_{2}_{3}_{4}{5}", _config.GetSection("FilePathUnzip:Path").Value, getStudentNumber.Value.ToString(), getInstituteName.Value.ToString(),
                   getStudentName.Value.ToString(), getCourseName.Value.ToString(), IsRunningLinux() ? "//" : "\\"));

                    var validation = await _mediator.Send(verifyFilesCommand);
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable.Add("files", validation.IsValid);
                    dictionaryToPassVariable["Worker"] = "verify_files";
                    camundaEngineClient.ExternalTaskService.Complete("FileLoading", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

            registerWorker("create_guid", async externalTask =>
            {
                try
                {
                    var delivery = externalTask.Variables;
                    var getId = returnVariableValue(delivery, "id");
                    var getStudentEmail = returnVariableValue(delivery, "studentEmail");
                    var getStudentName = returnVariableValue(delivery, "studentName");
                    var getInstituteName = returnVariableValue(delivery, "instituteName");
                    var getStudentNumber = returnVariableValue(delivery, "studentNumber");
                    var getCourseName = returnVariableValue(delivery, "courseName");

                    CreateFileIdentifiersCommand createFileIdentifiersCommand = new CreateFileIdentifiersCommand(Guid.NewGuid(), string.Format(@"{0}{1}_{2}_{3}_{4}{5}", _config.GetSection("FilePathUnzip:Path").Value, getStudentNumber.Value.ToString(), getInstituteName.Value.ToString(),
                   getStudentName.Value.ToString(), getCourseName.Value.ToString(), IsRunningLinux() ? "//" : "\\"), getStudentEmail.Value.ToString(), _config.GetSection("Notification:Email").Value);


                    var validation = await _mediator.Send(createFileIdentifiersCommand);
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable["ok"] = validation.IsValid;
                    dictionaryToPassVariable["Worker"] = "create_guid";

                    camundaEngineClient.ExternalTaskService.Complete("FileLoading", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

            registerWorker("send_broker", externalTask =>
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
                    var getId = returnVariableValue(delivery, "id");


                    Core.Model.DeliveryFileSystem deliveryFileSystem = new Core.Model.DeliveryFileSystem(Guid.Parse(getId.Value.ToString())
                        , getStudentName.Value.ToString(), getInstituteName.Value.ToString(), getCourseName.Value.ToString(), getStudentEmail.Value.ToString(), getStudentNumber.Value.ToString(), DateTime.Parse(getDeliveryTime.Value.ToString()),
                        getCordenatorName.Value.ToString(), getTitle.Value.ToString(), getDefenition.Value.ToString(), getpublicdefenition.Value.ToString(), getprivatedefenition.Value.ToString(), string.Format(@"{0}{1}_{2}_{3}_{4}{5}", _config.GetSection("FilePathUnzip:Path").Value, getStudentNumber.Value.ToString(), getInstituteName.Value.ToString(),
                   getStudentName.Value.ToString(), getCourseName.Value.ToString(), IsRunningLinux() ? "//" : "\\"));


                    bool validation = _queueService.SendToQueue(deliveryFileSystem, "PaymentProcessing");
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    dictionaryToPassVariable["ok"] = validation;
                    dictionaryToPassVariable["Worker"] = "send_broker";

                    camundaEngineClient.ExternalTaskService.Complete("FileLoading", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });


            registerWorker("notify_student_fileverifier", externalTask =>
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

                    _notificationService.SendEmail(getStudentEmail.Value.ToString(), string.Format("Olá, <br/> Não foram encontrado ficheiros públicos ou privados na tua entrega. Porfavor, contacta a tua Universidade! <br/> Obrigado, <br/> Delivery System."));
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    camundaEngineClient.ExternalTaskService.Complete("FileLoading", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });



            registerWorker("notify_student_loading", externalTask =>
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

                    _notificationService.SendEmail(getStudentEmail.Value.ToString(), string.Format("Olá, <br/> Ficheiros corrompidos foram encontrados. Porfavor, contacta a tua Universidade! <br/> Obrigado, <br/> Delivery System."));
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    camundaEngineClient.ExternalTaskService.Complete("FileLoading", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });

            registerWorker("manual_processing_loading", externalTask =>
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

                    _notificationService.SendEmail(_config.GetSection("Notification:Email").Value, string.Format("Ol]a, <br/> Algo correu mal na entrega. <br/> <br/> A entrega falhou no diagrama FileLoading. <br/> <br/> Nome do estudante:{0}, Nome da Universidade: {1}, Número do estudante: {2}, Nome do Curso:{3}. <br/> <br/> Ele falhou na fase {4} . <br/> <br/> Obrigado",
                        getStudentName.Value.ToString(), getInstituteName.Value.ToString(), getStudentNumber.Value.ToString(), getCourseName.Value.ToString(), getWorker.Value.ToString()));
                    Dictionary<string, object> dictionaryToPassVariable = returnDictionary(delivery);
                    camundaEngineClient.ExternalTaskService.Complete("FileLoading", externalTask.Id, dictionaryToPassVariable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _log.Fatal(ex.ToString());
                }
            });
            pollingTimer = new Timer(_ => StartPolling(), null, Convert.ToInt64(_config.GetSection("TimeToFetch:Time").Value), Timeout.Infinite);

        }

        private bool IsRunningLinux()
        {
            return _config.GetSection("FilePathUnzip:Path").Value.Contains("/");
        }

        private void DeleteFiles(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
        }

        private Dictionary<string, object> returnDictionary(Dictionary<string, Variable> delivery)
        {
            Dictionary<string, object> valuesforNextIteration = new Dictionary<string, object>();
            foreach (var item in delivery)
                valuesforNextIteration.Add(item.Key, item.Value.Value);
            return valuesforNextIteration;
        }

        private void registerWorker(string topicName, Action<ExternalTask> action)
        {
            workers.Add(topicName, action);
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
                var tasks = camundaEngineClient.ExternalTaskService.FetchAndLockTasks("FileLoading", Convert.ToInt32(_config.GetSection("TaskPerFetch:Tasks").Value), workers.Keys, long.Parse((Convert.ToInt64(_config.GetSection("TimeToFetch:Time").Value) / 2).ToString()), null);
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
    }
}
