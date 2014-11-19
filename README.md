FrameworkGL
======

These are my studies on modern OpenGL (3.3+) through [OpenTK](www.opentk.com) (an OpenGL library for C#)

####This work was based on many tutorials found on the internet:
* Simple Shader Tutorial: http://www.opentk.com/node/3693
* GL 3.1 tutorial: http://www.opentk.com/book/export/html/1039
* GLSL 1.2 Tutorial: http://www.lighthouse3d.com/tutorials/glsl-tutorial/?pipeline
* Tutorials for modern OpenGL (3.3+): http://www.opengl-tutorial.org/
* Vertex Buffer Objects: http://antongerdelan.net/opengl/vertexbuffers.html
* OpenGL Programming: http://en.wikibooks.org/wiki/OpenGL_Programming
* **Learning Modern 3D Graphics Programming: http://arcsynthesis.org/gltut/index.html**

This last tutorial is highlighted from the others intentionally: they are in different levels. Not that the other ones are bad, they're OK. But the detail given in the last one, and all the explanations of not just how to get things working, but the real reason of things, sets it apart from the others. Therefore, this will be the main tutorial I will be following for my further studies (until some other tutorial is needed).

Note that most of these tutorials don't say anything about OpenTK or C#. They are mostly based in C++ using a few external library. This is not an issue because OpenTK manages the window through the GameWindow class (which my GameMain class derives from), and all the functions have the same names and parameters, only adjusted to C# (for example, no more GLtype data types (regular types) or GL_HUGE_NAME_CONSTANT constants (enum), no more glFunctionName (GL.FunctionName)). It also works with vectors and matrices (built-in), and has enough XML documentation.

This repository will be used for the implementation of my assignment for the Graphics module at the University of Lincoln.