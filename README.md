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


Learning
======

Custom result transformation
======

Thanks
======
