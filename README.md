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

# Design

## Caching

- Uses an in memory cache. If it was scaled out, could move to a distributed cache
- Doesn't store IP locations when the lookup was not found by the third party. The IP address could be allocated at a later date

## Persistent store

- Uses MongoDB as it's simple to get up and running, and has Docker support
- A unique index on the IP address is added using a hosted service on start up. This should ideally be done by a migrations tool so that the application user doesn't need elevated privileges 

## Unit tests

- These are focused around the cache behaviour as this is difficult to test in an integration test
- I likely wouldn't use Moq in the real world and would connect to a MongoDB instance. Moq was used here for simplicity

## Integration tests

- These are focused around the vertical slice of getting an IP location
- We verify that IP v4 and v6 can be looked up
- We verify that an invalid IP returns a 400
- We verify that when an IP isn't found it returns a 404
- We verify that an IP that is found is returned as we expect
- We verify that an IP that is found is stored as we expect in MongoDB