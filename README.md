

## Ports 

### Infrastructure

* Database:
  * 9003:5432
* RabbitMQ:
  * 9007:5672
  * 9008:15672

### Services

* Auth
  * 9010:80
  * 9011:443
* Products
  * 9005:8080
* Orders
  * 9006:8080
* Notifications
  * 9009:8080

### Clients

* EmployeesClient
  * 9002:8080
* NextJS
  * 9001:3000
* Gateway
  * 9000:80
  * 9004:443