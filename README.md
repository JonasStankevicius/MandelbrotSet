## Task ##

Generate Mandelbrot set vizualization.  
User should be able to zoom into specific sections of the set.  

![alt text](docs\\full_mandelbrot.png)
![alt text](docs\\zoom_mandelbrot.png)

## Requirements ##

Each frame of the vizualization should be generated in less than second to provided somewhat smooth zoom in/out transitions.  
Each frame on zoom in/out should be centered at the previously picked point to show seamless zoom in/out transition.  

## Use cases ##

![alt text](docs\\mandelbrot_use_case.png)

User can use the mouse to point at some part of the set and zoom in with scrool button.  
Zoom out also centers frame at the pointed part of the set.  

## Implementation ##

WPF is used to vizualize generated raw **621x624** RGB image.  
Each pixel gets x and y values from range [-2;2]. Zooming in narrows this range to a minimum of [-0.05;0.05].
Then fractal function is applied for each pixel 50 times.  
Whole image is divided into **50x50** tiles (**169** tiles in total) and all of those are computed in parallel.  

```
Image generated = 00:00:00.2708824
Image generated = 00:00:00.2921067
Image generated = 00:00:00.2671910
Image generated = 00:00:00.2951162
Image generated = 00:00:00.2980003
Image generated = 00:00:00.2856885
Image generated = 00:00:00.2681400
```

Each frame is generated in less than 1/3 second.  

## Documentation ##

Technical documentation is provided in the folder "docs".
Run **"docs\index.html"** file in your internet browser.
