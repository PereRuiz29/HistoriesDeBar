VAR opcio1 = true
VAR opcio2 = true
VAR opcio3 = true
VAR opcio4 = true

Buenos días, quería un... un... ¿Cómo se dice? #speaker:JoséMaría
* {opcio1} [¿Un café corto?]
    ~ opcio1 = false
* {opcio2} [¿Un café con leche?] 
    ~ opcio2 = false
* {opcio3} [¿Un wisky?]
    ~ opcio3 = false
* {opcio4} [¿Un carajillo?]
    ~ opcio4 = false
    
-No, no, no. Quería un...
* {opcio1} [¿Un café corto?]
    ~ opcio1 = false
* {opcio2} [¿Un café con leche?] 
    ~ opcio2 = false
* {opcio3} [¿Un wisky?]
    ~ opcio3 = false
* {opcio4} [¿Un carajillo?]
    ~ opcio4 = false

-¡Que no, hostia! Es un... emm...¡
* {opcio1} [¿Un café corto?]
    ~ opcio1 = false
* {opcio2} [¿Un café con leche?] 
    ~ opcio2 = false
* {opcio3} [¿Un wisky?]
    ~ opcio3 = false
* {opcio4} [¿Un carajillo?]
    ~ opcio4 = false

-Te he dicho que no!

* {opcio1} [¿Un café corto?]
    #order:curt
* {opcio2} [¿Un café con leche?]
    #order:cafeAmbLlet
* {opcio3} [¿Un wisky?]
    #order:whisky
* {opcio4} [¿Un carajillo?]
    #order:rebentat

-Eso, hostia! joder, mira que te ha costado.
Y que sea rápido, ¿eh?
 
