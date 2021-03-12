# mongodb-change-stream-test

Just a demo for the MongoDB C# driver Change Stream features/syntax.

## Change Stream

Change Stream is a Mongo DB feature that allows for a client to connect to the database and the database will push changes to the client.  
This is useful when you need your clients to be informed of database changes and you don't want to be polling the database every 5 minutes or so.
Simply connect to the database and wait for the changes to come in.
You can also modify the way you scan for changes, for example listen only for inserts instead of all changes.