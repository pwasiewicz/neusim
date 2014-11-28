neusim
======

 Command-line app to simulates multilayer neuron network. There are three layers so far: input, hidden and output, so should be enough to approximate lots of function.

Features
======
* git like interface
* learning with backpropagation algorithm
* applying custom function to transform / interpret output (via javascript)
* JSON input

! Interface
NeuSim contains git like interface (verbs) with the following commands:
   * **init**:            Inits new simulator session inside working directory.
   * **config**:       Allows to specify configuration of current session.
   *  **destroy**:     Destroys session inside working directory if available.
   * **simulate**:    Simulates the specified data with neuron network
   * **learn**:         Learn the network of with specified data.
   * **display**:      Displays the neuron network

! Learing

! Custom result transformation
