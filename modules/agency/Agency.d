﻿Person protocal { 
  contract  (contractor : Person,                          terms  : [ ] terms) -> Contract       
  employee  (employee   : Human,                           terms  : [ ] terms) -> Employment 
  purchase  (asset      : Asset | Product,                 terms  : [ ] terms) -> Purchase 
  bill      (entity     : Person, lines: [ ] Invoice`Line, terms? : [ ] terms) -> Invoice

  contracts    -> [ ] Contract      // Contracts(contractee).contractor
  employments  -> [ ] Employment    // Employments(employer).employee
}

// maybe...
, Municiplaity
, County      
, Country : Organization { 

}

Purchase ≡ Asset

// via invoicing::Invoices(owner);						// bill an Entity				

// via finance::Stock(owner);							// own shares of an Entity
// via Assets(owner);

reserve 1_000_000_000_000 records for Humans
reserve 1_000_000_000_000 records for Organizations

// { relator: Entity, relatee: Entity }

type Bylaw = law::Rule | law::Regulation | law::Term:

// Organization 


// Employee of X
// Member of

Dog(Blue) is dead   // true
Dog(Pony) is alive  // true

when did blue die?

Entities partioned by kind thenby registar;

reserve 1000000000000 records for Organisms;
reserve 1000000000	  records for Humans;
reserve 1000000000000 records for Organizations;
reserve 1000000000    records ∀ Birth.registrar;
reserve 1000000000    records ∀ Incorporation.registrar;