# Technical Spec

Design a micro-service that would use (wrap, really) any of the 3rd party location-from-IP address service. The service that you’ll build must expose a REST interface.

- Service should maintain a cache 
- Service should maintain a persistent store of the looked-up values
- Unit tests
- Integration tests
- It may use Swagger (or Postman) as a UI.

# Running the app

1. Clone the repo
2. Run `docker compose up --detached` to start the service
3. Navigate to http://localhost:5000/swagger
4. Use the swagger UI to test the service