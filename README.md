# Segundo Projeto - Entregas de Projetos.

## Tema

O segundo projeto terá como objetivo a criação de uma solução digital de entrega de trabalhos finais de estudantes de licenciatura, mestrado e doutoramento (tese, dissertação, estágio ou projeto), para o ISMAI e IPMAIA, processando as entregas, notificando os alunos e a Instituição em cada etapa do processo.

Para tal vão ser criadas três fases, assim como um website desenvolvido para permitir a entrega dos alunos, com a possibilidade de entrega por link. 

As fases do projeto terão como nome FileLoading, Payment e FileProcessing. 

Ao dividir este objetivo por três fases, a complexidade vai ser reduzida, a responsabilidade de cada fase vai estar bem definida e vai haver um maior controlo sobre cada fase.

Com este projeto, o pretendido passa por inovar e simplificar o processamento manual, sendo que tudo está pensado por forma a recorrer apenas a um recurso humano, que vai intervir caso surjam falhas.

## Arquitetura da solução

Nesta arquitetura podemos verificar que o RabbitMQ assume uma grande importância, juntamente com o Camunda BPM.

Caso não consigamos enviar mensagens com o RabbitMQ, vai ser gerado um erro com a informação e o sistema vai enviar o processo para validação manual. No caso de erro, o aluno vai ser alertado e aconselhado a repetir a submissão.

Caso o Camunda BPM falhe, vai ser gerado um erro e a informação não vai ser obtida, contudo o sistema irá continuar a tentar processar entregas.


## Vídeo



[![IMAGE ALT TEXT](http://img.youtube.com/vi/pDoP1SKzdwA/0.jpg)](https://youtu.be/pDoP1SKzdwA "Segundo Projeto")


Autor: Carlos Campos
