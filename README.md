# MongoDbPoC 
Repository Pattern implementation in C# for MongoDB. <br /><br />
This solution doesn't have a UI or API implementation. Only the repository and test libraries are present. <br />
<ul>
  <li>MongoDbPoC.Data - Repository pattern implementation.</li>
  <li>MongoDbPoC.Tests - Integration tests.</li>
</ul>
<b><i>Testes are intentionally not mocked.</i></b>
<br /><br />
<b>Requirements:</b> Valid MongoDB instance to establish a connection.<br /><br />
<b>Docker</b>
<br />
A Docker can be used executing the following command:<br />
<i>docker run -d --name MongoDbService -p 3017:27017 -e MONGO_INITDB_DATABASE=MongoDbLab mongo</i>
