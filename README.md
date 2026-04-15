# architecture-lab-event-driven

## 📌 Architecture Overview

This project demonstrates an **event-driven order processing system** using the **Outbox Pattern** to ensure reliable message delivery.

The system is designed around three main components:

- **Order API**: Receives incoming requests and stores both the order and the corresponding event in the database within a single transaction.
- **Outbox Publisher**: Continuously scans the Outbox table and publishes pending events to RabbitMQ.
- **Order Processor**: Consumes events from the message queue and executes the business logic, updating the order lifecycle.

---

## 🔄 Processing Flow

flowchart TD
    client[Client / Frontend] --> api[Order API]

    api --> validate[Validate Command]
    validate --> order[Create Order Entity]
    order --> outbox[Create Outbox Message]
    outbox --> tx[(Single Transaction)]

    tx --> db[(Orders + Outbox DB)]

    db --> publisher[Outbox Publisher Worker]
    publisher --> scan[Scan Pending Outbox Messages]
    scan --> publish[Publish OrderCreated Event]
    publish --> rabbit[(RabbitMQ)]
    publish --> sent[Mark Message as Sent]

    rabbit --> processor[Order Processor Worker]
    processor --> consume[Consume Event]
    consume --> processing[Set Order Status = Processing]
    processing --> business[Execute Business Logic]
    business --> completed[Set Order Status = Completed]

    completed --> db
    sent --> db
    
1. The client sends a request to create a new order.
2. The API validates the request and stores:
   - The `Order` (with status `Pending`)
   - The `OutboxMessage` (event: `OrderCreated`)
   in the **same database transaction**.
3. The Outbox Publisher periodically reads unsent messages from the database.
4. Each message is published to RabbitMQ and marked as **Sent**.
5. The Order Processor consumes the event from the queue.
6. The order is processed and its status transitions:
   - `Pending → Processing → Completed`

---

## ⚙️ Key Architectural Concepts

- **Event-Driven Architecture**: Services communicate through events instead of direct calls.
- **Outbox Pattern**: Guarantees that no events are lost during the transaction.
- **Asynchronous Processing**: Decouples the API from background processing.
- **Eventual Consistency**: Data is not immediately consistent but becomes consistent over time.

---

## 🎯 Purpose

The goal of this project is to demonstrate how to design a **reliable, scalable, and loosely coupled backend system** using modern architectural patterns commonly found in distributed systems.




