# Simple pathfinding

![alt text](https://github.com/adavattedeve/SimplePathFinding/edit/master/Pathfinding.gif "pathfinding")

# Container for open set

I used binary min heap as container for the open set due to it being fast to implement and yet very effective priority queue. 
Comparing to manually sorting traditional container of objects, the performance of binary heap scales expotentially better.
I tested also pairing heap which should have even better running times, but my test results didn't show any consistent improvements.
Pairing heap had also one other problem why I didn't put too much time trying to optimize it. It doesn't have any performative way to update value in it and that is needed when recalculating the F value for node that is already in the open set.

# Heuristics

 I decided to use distance as heuristics. It is admissible so the path is guaranteed to be shortest possible and it is still a lot faster than anyting lower like breadth-first search where h is always 0 . I also tried using weighted distance as it offers in most cases faster running times, 
but in the end I chose precicion over speed because the grid was so small. Choosing right heuristics is hard without knowing where it will be used because the unit movement style, obstacle layout 
and performance requirements have huge impact to the final decicion. As an afterthought, I think that overall best solution would have been to use some kind of precomputed distance data as heuristics.

# References

https://en.wikipedia.org/wiki/A*_search_algorithm

https://en.wikipedia.org/wiki/Admissible_heuristic

https://en.wikipedia.org/wiki/Priority_queue

https://en.wikipedia.org/wiki/Binary_heap

https://en.wikipedia.org/wiki/Pairing_heap
