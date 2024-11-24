### Integration testing

The initial integration testing setup two mocks and achieved in testing 2 additional lines of code. It fails in adding value and the maintainability is questionable.

Refactoring into integration tests that use real database is a compromise that gains in value but loses in execution time.

As the application can only write values to database, and at this point adding functionality to BuyerRepository to satisfy testing is a code smell, the functionality as added to a test helper class. To implement functionality tests need reference some lightweight ORM (object relational mapper) such as Dapper.

links:

[Eradicating non-determinism in tests](http://martinfowler.com/articles/nonDeterminism.html)

[Unit testing pattern for concurrent code](https://vimeo.com/171317257)