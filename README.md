# gron-mdt
gron-mdt; make markdown-table greppable.
inspired by https://github.com/tomnomnom/gron
 
## install

TODO

## usage

### for sample 

|Name |Quantity|
|-----|--------|
|Apple|3       |
|Egg  |12      |

```
$ curl -s https://raw.githubusercontent.com/mitakeck/gron-mdt/master/README.md | gron-mdt
json = {};
json.Name = {};
json.Apple = {};
json.Apple.Name = "Apple";
json.Apple.Quantity = 3;
json.Egg = {};
json.Egg.Name = "Egg";
json.Egg.Quantity = 12;
$
```


