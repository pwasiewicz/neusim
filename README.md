neusim
======

 Command-line app to simulates multilayer neuron network. There are three layers so far: input, hidden and output, so should be enough to approximate lots of function.

Features
======
* git like interface
* learning with backpropagation algorithm
* applying custom function to transform / interpret output (via javascript)
* JSON input

Interface
======
NeuSim contains git like interface (verbs) with the following commands:
   * **init**:            Inits new simulator session inside working directory.
   * **config**:       Allows to specify configuration of current session.
   *  **destroy**:     Destroys session inside working directory if available.
   * **simulate**:    Simulates the specified data with neuron network
   * **learn**:         Learn the network of with specified data.
   * **display**:      Displays the neuron network

init
-----------
  `-i, --input` 
  
  Required. Number of input neuron of network. Sample: **-i 2**
  
  `-h, --hidden` 
  
  Required. Number of neurons in hidden layer. Sample: **-i 2**

config
-----------
  `-a, --activation`   
  
  Sets the activation function for neurons. Supports base function like Exp, Sin or Cos. The variable is x literal. Sample: **-a 1/(x+Exp(-x))**

  `-d, --derviative`    
  
  Sets the derivative of activation function for neurons.  The variable is x literal. Sample: **-d x*(1-x)**

  `-p, --parser`       
  
  Allows to the script that will be applied to result  network. The input is file name with parser function specified. Sample: `-p result-parser.js`

  `-e, --epoch`         
 
 Sets the number of epoch used in learn properties.  Sample: **-e 10000**

 `-t, --tolarance`
  
  Sets the tolerance of error on output value. Sample: **-t  0.005**

  `-w, --weight`
  
  Sets manually weight of specified input in neuron inside  layer. Sample (first layer, first neuron and first  input): **-l 1 -n 1 -i 2 -w 1.02**

 `-b, --bias`          
 
 Sets manually bias of specified input in neuron inside layer. Sample: **-l 1 -n 1 -i 2 -b 0.02**

  `-l, --layer`         
  
  Sets the context layer for setting weight or bias.

  `-n, --neuron`        
  
  Sets the context layer for setting weight or bias.

  `-i, --input`         
  
  Sets the context for input of selected neuroon in "neuron" option.

Simulate
-----------

  `-f, --files`       
  
  Simulates data from specified files. Sample: **-f File1 File2**

  `-i, --input`        
  
  Simulates the data given in standard input. Sample: **-i 0.2  0.4**

  `--aggregate`        
  
  (Default: False) Applies custom aggregate function to results. Transform file must be specified.

  `--skiptransform`    
  
  (Default: False) Skips transform if available.
  
### Format of input files
   The input files are simple json array wtih **double** values of input neuron. Sample (three inputs):
   
   `[1.2, 1.3, 1.3]`
  
Learn
-----------

  `-p, --path`
  
  Learns network from files inside specified path. Files must end  with end with learn extension. Sample: **-p LearnCases\XOR**

  `-f, --file`    
  
  Learns the specified learn case from file. Sample **-f LearnCase.learn**

  `--all`
  
  Learns all non-learnt cases from all subdirectories.

  `--force`       
  
  Flag that forces to learn cases even it has already been  learnt.

Learning
======

Learng file is simple JSON object that represents any number of learn cases. Learn case consists of input and expected output. Neusim uses back propagation algorithm to learn network. Sample learn case of XOR function:
```javascript
[
   {
      "Output":1,
      "Input":[
         1,
         0
      ]
   },
   {
      "Output":1,
      "Input":[
         0,
         1
      ]
   },
   {
      "Output":0,
      "Input":[
         0,
         0
      ]
   },
   {
      "Output":0,
      "Input":[
         1,
         1
      ]
   }
]
```

Custom result transformation
======
You can apply transform function in order to interpret result:
```javascript
function transform(x) {
  if (x < 0.5) {
     return "yes";
  } else {
     return "no";
  }
}
```

The sample script you should put in js file (js extension is not required) and set it in config:

```ShellSession
PS> neusim config --parser myscriptfile.js

```


Then simulating will print output transformed:

```ShellSession
PS> neusim simulate -i 1 0
yes

```

Thanks
======
Thank to all contributor of the following libraries used in this project:
* Autofac
* Command Line Parser Library
* Json.NET
* Jint - Javascript Interpreter for .NET
* NCalc - Mathematical Expressions Evaluator for .NET
