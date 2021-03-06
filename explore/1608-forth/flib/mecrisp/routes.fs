\ Routing for definitions to replace all instances of a definition with a new one.
\ Very slow and bulky... Intended to use for debugging only.

\ Disassembler required for printing a routing table.

\ -----------------------------------
\  A few tools for dictionary magic
\  Taken from profiler
\ -----------------------------------

: codestart>link ( addr -- addr* )
  >r

  dictionarystart
  begin
    dup 6 + count + even r@ = if rdrop exit then
    dictionarynext
  until
  drop

  r> \ Not found ? Then give back the address which we had searched for.
  ."  Could not find header for code start address " hex. ." in dictionary." cr quit
;

: link>codestart ( addr -- addr* )
  6 + count + even
;

 \ Link is -1 or 0 ? This is end of dictionary.
 \ Link is higher than current address ? Flash.
 \ Link is lower  than current address ? RAM.

: dictionarybefore ( addr -- addr* ) \ Get one older definition, if possible. Complicated, as link may be set in both ways.

  dup dictionarynext nip 0= \ Check if end of link chain is reached to not break idea of u>. Unset links can have different values depending on target !
  if
    dup dup @ ( Current Current Next ) u> if @ exit then \ RAM
  then

  >r \ Flash.
  dictionarystart
  begin
    r@ over dictionarynext drop = if rdrop exit then \ Found ? Give back the definition before.
    dictionarynext
  until
  drop
  r> \ Not found ? Then give back the address which we had searched for.
;

\ ----------------------------
\  Routing logic and insight
\ ----------------------------

: routes ( -- ) \ Print a complete routing table. Check all definitions available if they have a routing vector, and print those who are currently rerouted.
  cr
  dictionarystart
  begin

    dup dictionarybefore dup 6 + count s" (vec)" compare
    if
      link>codestart execute @ ?dup
      if
        over 6 + ctype name. cr
      then
    else
      drop
    then

    dictionarynext
  until
  drop
;

: getvector ( xt -- addr )
  codestart>link
  dictionarybefore
  dup 6 + count s" (vec)" compare
  if
    link>codestart execute
  else
    drop
    ." No routing vector for this definition" cr
    quit
  then
;

: route ( -- ) \ Usage: route old-definition new-definition
  ' getvector
  '
  swap !
;

: unroute ( -- ) \ Usage: unroute name
  ' getvector
  0 swap !
;

\ ----------------------
\  A new routing colon
\ ----------------------

: : ( -- )
  s" 0 variable (vec)" evaluate
  :
  s" (vec) @ ?dup if execute exit then" evaluate
;

\ All definitions from now on will be prepared for routing.
