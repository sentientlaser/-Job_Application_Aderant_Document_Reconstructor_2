# Document Reconstructor

# Project Structure

Project is in two files:
- `DocumentReconstructor.cs` contains the interface and implementation of the algorithm.
- `DocumentReconstructorTests.cs` contains simple Nunit tests for against the interface (not directly against the implementation, see below)

Both the tests and the Interface / Implementation are in the same namespace to save typing and namespace conflicts / omissions.  Usually I'd place tests in the same namespace but in a different source set (or project), but like a few other things in this project I've kept it simple.

# Solution Strategy

Problem has been apprached in a fairly simple Test Driven Design method.
- An interface is created, then tests against the interface written.
- Implementation is provided after the interface is specified and the tests written.

A few assumptions have been made
- Result is not known ahead of time (reduces to a trivial case)
- Results are well defined (there's a purely algorithmic solution, heuristic solutions would require more test data to establish accuracy)
- There are no instances of fragments where there are two equally viable selections to be concatenated (becomes a heuristic problem)

# Implementation Design

The implementation is pretty flexible, and different algorithms can be specified with different pair selection functions.

# Code Notes

I've mostly used tuples to save creating intermediate class types for aggregated data.

I've also used C# idioms in a fairly inconsistent manner.  Usually I don't do this in enterprise / large scale code because it's a hindrance to comprehension and static analysis.  This is really just a 'flex' on my part: since this is a job application I want to show I'm comfortable with different idoms.

# Test Data Notes

The test data contains one small point that will not allow simple string intersection based algorithms to complete.  While usually something to be avoided this datum has been altered and the change noted.
