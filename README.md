# Beziercurve_Interactive
I implemented a simple Tool for Students who wants to inspect Beziercurves.
This Tool have just only one spline with two curvesegments each parameterizable to grade n=27

### 1. you can inspect the flexibility of a curve with higher grades
### 2. continuity of GC0, GC1 and GC2 is also inspectable with limited degrees of freedom for each continuity
### 3. restrictions of continuity will show you the attributes of tracks contructed with beziercurves
- - -
## Control

## With the Bezier curve handle selected
| Keyboard | Effect |
| :------- | :------: |
| Space | deletes the control point Pn |
| RMB | create controlpoint |

## Camera control
| Keyboard | Camera driving direction/Effect |
| :------- | :------: |
| W | forward |
| A | left |
| S | backward |
| D | right |
| E | down |
| Q | up |
| Shift + LMB | Look around |

## Linking the Bezier curves - Spline generation -
- Basically, a spline can only be created with the **last point** of the **first curve** and the **first point** of the **second curve**.
- This becomes clear when the levels are switched on 
- Repeated pressing of the number keys toggles the **switching on and off the transition continuity**


| Keyboard | Effect |
| :------- | :------: |
| Ctrl + LMB + Move point Pn of the first curve into P0 of the second curve | generates GC0 continuity |
| NumberKey 1 (not numeric keypad) | generates GC1 continuity |
| NumberKey 1 (not numeric keypad) | generates GC2 continuity |
| NumberKey 1 | dissolves the spline and thus also the curve transition  |

### The rest of the setting options are provided by a minimal UI
