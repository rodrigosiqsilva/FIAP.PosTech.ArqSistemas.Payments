# FIAP.PosTech.ArqSistemas.PaymentsWS

Serviço de background (**Worker Service**) desenvolvido em **.NET 8** responsável pela orquestração, processamento e liquidação financeira de pedidos dentro do ecossistema de microsserviços. 

O componente atua de forma reativa e assíncrona utilizando o **Apache Kafka** para escutar novas intenções de compra, acionar APIs de negócio para atualização de estado e difundir a confirmação de pagamento processado para liberar os produtos adquiridos.

---

## 🛠️ Tecnologias e Frameworks

* **Runtime:** .NET 8.0 (BackgroundService)
* **Mensageria (Consumo):** Confluent Kafka Client (Event-Driven Consumer - `OrderPlaced`)[cite: 36, 41]
* **Mensageria (Publicação):** Confluent Kafka Client (Event-Driven Publisher - `PaymentProcessed`)[cite: 36, 42]
* **Comunicação HTTP:** HttpClient (Injeção de dependência e chamadas REST com tratamento de resiliência)[cite: 39]
* **Containers & Orquestração:** Docker & Kubernetes

---

## 🎯 Escopo e Funcionamento Interno

Este Worker opera como o intermediador do fluxo de transações da plataforma, executando as seguintes etapas a cada evento de checkout:

1. **Assinatura de Entrada (`KafkaConsumerWorker`):** O serviço de background mantém um loop de escuta contínuo assinado no tópico configurado de pedidos realizados (`OrderPlacedEventCreated`).
2. **Isolamento de Escopo (`OrderPlacedEventConsumer`):** Para cada mensagem capturada do Kafka, o Worker cria um escopo de injeção temporário (`IServiceProvider.CreateScope`) para instanciar os serviços de integração HTTP com total segurança concorrente.
3. **Liquidação e Atualização (`IOrderGameService`):** O Worker dispara uma requisição HTTP PUT para a API de gerenciamento de ordens invocando o método `AlterarStatusAsync`, atualizando o status do pedido para `Approved` (Aprovado).
4. **Disparo de Confirmação (`PaymentProcessedEventPublisher`):** Uma vez confirmada a transação pela API, o Worker monta o evento de sucesso `PaymentProcessedCreatedEvent` e publica a mensagem usando o `Id` do pedido como chave de partição do Kafka, garantindo a ordenação cronológica dos eventos daquela compra.

---

### Repositório do Ecossistema
Você precisará clonar o seguintes repositório do projeto:

| Repositório | Link para Clone |
| :--- | :--- |
| **Payments WS** | `https://github.com/rodrigosiqsilva/FIAP.PosTech.ArqSistemas.Payments.git` |

### 📂 Estrutura de Pastas Obrigatória
Para que os arquivos de orquestração local (Docker Compose) referenciem os projetos corretamente, você **deve** respeitar a seguinte estrutura de diretórios no seu disco:

Veja um exemplo através da imagem: https://github.com/rodrigosiqsilva/FIAP.PosTech.ArqSistemas.Orchestrator/blob/main/Estrututa%20pastas.png

```text
C:\Sistemas\FIAP\     
├── FIAP.PosTech.ArqSistemas.Catalog/  
├── FIAP.PosTech.ArqSistemas.User/
├── FIAP.PosTech.ArqSistemas.Notification/
└── FIAP.PosTech.ArqSistemas.Payments/ <- (Arquivos desse repositório mencionados aqui)