// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

(*
Remember:
    1. Identify the user(s) of the program
    2. Make sure you have understood the basic requirements
        2.1 What is the program supposed to do?
        2.2 What style of interaction is appropriate for the program based on the intended user(s)?
    3. Think about how the program should behave from the user's perspective
        3.1 Work through some "stories" of how the program might be used for its intended purpose
            3.1.1 More interactive programs will have more complex stories
        3.2 Think about where things could go wrong, where could errors occur and why?
            3.2.1 Work through some "stories" of how the user might use the program incorrectly and what happens if they do
        3.3. Provide examples of inputs and outputs to the program (if I give it: this, that, another thing I should get back: expected result
    4. Think about the basic "algorithm" the program should follow
    5. Break the algorithm down into further and further details with greater precision for each step
    6. Once you have a pretty clear structure 
       that is relatively precise with what should happen at each stage, 
       start turning each step into code
    7. With each step you just need to focus on implementing THAT STEP - worry about the later steps when you get to them
    8. Once you have the basic program structure, test it out based on the example inputs and outputs you created


User:
    Technically proficient user using a batch script for automation
    Technically proficient user (via the terminal/command line)

Requirements:
    Purpose:
        Program should add three integer values together
    Interactivity: 
        The program should not require user-interaction past passing the arguments in to the program at the beginning
        (thus making it easier to use via scripts)


Behaviour:
    I/O:
        AddingThreeIntsTogether1 0 1 2 = 3
        AddingThreeIntsTogether1 1 2 3 = 6
        AddingThreeIntsTogether1 1 2 = ERROR: Invalid amount of arguments, expected 3 arguments but got 2
        AddingThreeIntsTogether1 1 2 3 4 = ERROR: Invalid amount of arguments, expected 3 arguments but got 4
        AddingThreeIntsTogether1 1 hello 3 = ERROR: One or more arguments were invalid, each argument should be a valid integer


Steps (breaking down our program):
    // First version (general overview/summary)
    Adding three integers together via the command line

    // Second version (a bit more detail)
    Get arguments to program
    if amount of arguments is correct
    then
        try to convert each string to an integer
        if all are valid integers
        then
            print out the result of adding them together
            return success exit code
        else
            Output error that tells us invalid arguments were provided
            return error exit code
    else
        Output error for incorrect amount of arguments
        return error exit code

    // Third version (further detail)
    if program argument array length is 3
    then
        Extract each string from the array and try to parse them to an integer
        if each string can be a valid integer
        then
            add together each of the integer values
            print out the result
            return a success error code
        else
            ERROR: One or more arguments were invalid
            return an error exit code
    else
        ERROR: Invalid amount of arguments, expected 3 arguments but got <length of array>
        return an error exit code

    // Fourth version (breaking it apart even more, some part now look even more like pseudo-code)
    if result of Array.length argv is 3
    then
        Extract each string from the array
        Try to parse each of the strings
        if the three arguments (x, y and z) are valid integers
        then
            print <x+y+z>
            return an exit code indicating success (success error code is 0)
        else
            print "ERROR: One or more arguments were invalid"
            return an exit code indicating an error occurred (this is a non-0 integer)
    else
        print "ERROR: Invalid amount of arguments, expected 3 arguments but got <Array.length argv>"
        return an exit code indicating an error occurred (this is a non-0 integer)


FROM ALGORITHM DESCRIPTION TO SOURCE CODE:
    // First version of the source code
    open System

    [<EntryPoint>]
    let main argv = // argv is our array of arguments from the command line
        // if program argument array length is 3
        if Array.length argv = 3
        then
            // Extract each string from the array and try to parse them to an integer
            let validX, x = argv.[0] |> Int32.TryParse
            let validY, y = argv.[1] |> Int32.TryParse
            let validZ, z = argv.[2] |> Int32.TryParse

            // if each string can be a valid integer
            if (validX && validY && validZ)
            then
                // add together each of the integer values
                // print out the result
                printfn "%i" (x+y+z)
                0 // return an integer exit code indicating success
            else
                // ERROR: One or more arguments were invalid
                printfn "ERROR: One or more arguments were invalid, each argument should be a valid integer"
                1 // return an integer exit code indicating an error occurred
        else
            // ERROR: Invalid amount of arguments, expected 3 arguments but got <length of array>
            printfn "ERROR: Invalid amount of arguments, expected 3 arguments but got %i" (Array.length argv)
            1 // return an integer exit code indicating an error occurred

    // Now think about how we can rewrite our code in different ways
    // Can we make it more compact?
    // Second version of the source code (replace nested if expressions with a match expression using guarded clauses)
    open System

    [<EntryPoint>]
    let main argv = 
        // Try to parse each of the strings
        let parsedArray = Array.map Int32.TryParse argv
        // if program argument array length is 3
        match parsedArray with
        |[|_, x;_, y;_, z|] 
            when Array.forall (fun (validInt,intValue) -> validInt) parsedArray ->
                // add together each of the integer values
                // print out the result
                printfn "%i" (x+y+z)
                0 // return an integer exit code indicating success
        |[|x;y;z|] ->
                // ERROR: One or more arguments were invalid
                printfn "ERROR: One or more arguments were invalid, each argument should be a valid integer"
                1 // return an integer exit code indicating an error occurred
        |_-> 
            // ERROR: Invalid amount of arguments, expected 3 arguments but got <length of array>
            printfn "ERROR: Invalid amount of arguments, expected 3 arguments but got %i" (Array.length argv)
            1 // return an integer exit code indicating an error occurred

    // The program is short enough for us to not need to worry about breaking it apart into many separate functions
    // However we could write a function to contain our match expression
    // We could also write another function which tries to parse each string in the array to an integer
    // We can also write a function which allows us to check that all the values of the array provide a valid integer
    // We can then have a simple pipeline in our main function
    // This will make the body of the main function easier to read
    // It will also mean that we can make a more easily testable, modular program (by breaking it into different functions)
    // Third version (break it apart into functions and start to build a pipeline)
    open System

    let checkAllAreValidInts args =
        // Check that the first element in each TryParse result
        // within the array is true (and thus each conversion attempt was successful)
        Array.forall (fun (validInt,intValue) -> validInt) args

    let checkInputAndSum parsedArray =
        // if program argument array contains 3 elements (each of which are tuples)
        match parsedArray with
        |[|_, x;_, y;_, z|] 
            when checkAllAreValidInts parsedArray->
                // add together each of the integer values
                // print out the result
                printfn "%i" (x+y+z)
                0 // return an integer exit code indicating success
        |[|x;y;z|] ->
                // ERROR: One or more arguments were invalid
                printfn "ERROR: One or more arguments were invalid, each argument should be a valid integer"
                1 // return an integer exit code indicating an error occurred
        |_-> 
            // ERROR: Invalid amount of arguments, expected 3 arguments but got <length of array>
            printfn "ERROR: Invalid amount of arguments, expected 3 arguments but got %i" (Array.length parsedArray)
            1 // return an integer exit code indicating an error occurred

    let parseInputArgumentsToInts argv =
        // Convert array of strings to array of tuples which are the results
        // of attempting to parse each of the elements in the array
        Array.map Int32.TryParse argv

    [<EntryPoint>]
    let main argv = 
        // Take argv, try to parse the input arguments to ints, then check the input arguments and sum them together
        argv |> parseInputArgumentsToInts |> checkInputAndSum

    // Not bad! But we can break the program apart even further
    // Notice that the "checkInputAndSum" not only checks that the input arguments are valid
    // (by using the "checkAllAreValidInts" function and breaking apart the array using pattern matching)
    // but it also prints out the result (or print if an error occurred)
    // Let's separate out the process of calculating the result / returning an error 
    // from the process of printing out the result/error and returning the exit code
    // this will allow us to separate our "impure" code from our "pure" code
    // remember that "impure" functions perform some kind of state change (like doing I/O)
    // Fourth version (break it apart further into pure and impure functions)
    open System

    // Creating a custom type to represent the different error messages
    // ErrorMessages can either be 'InvalidArgumentAmount (and the amount of arguments)
    // or 'ArgumentNotValidInt'
    type ErrorMessage =
        |InvalidArgumentAmount of int // How many arguments were provided?
        |ArgumentNotValidInt // Are one or more arguments invalid ints?


    // Creating a custom type to represent whether the input parsing of the arguments was valid or invalid
    // This custom type uses type variables 
    // which allow F# to work out which types 'TSuc and 'TMsg are automatically
    type Result<'TSuc, 'TMsg> = 
        | Ok of 'TSuc // The result is Ok (return the successful result)
        | Error of 'TMsg // The result is an error (return an error value)

    // This is a PURE function
    let checkAllAreValidInts args =
        // Check that the first element in each TryParse result
        // within the array is true (and thus each conversion attempt was successful)
        Array.forall (fun (validInt,intValue) -> validInt) args

    // This is an IMPURE function
    let printOutputAndGetExitCode result =
        // Handle the output correctly (based on the specific 'Result' value provided
        match result with
        |Ok(summedValue) ->
            printfn "%i" summedValue
            0
        |Error(ArgumentNotValidInt) ->
            printfn "ERROR: One or more arguments were invalid, each argument should be a valid integer"
            1
        |Error(InvalidArgumentAmount(argumentAmount)) ->
            printfn "ERROR: Invalid amount of arguments, expected 3 arguments but got %i" argumentAmount
            1

    // This is a PURE function
    let checkInputAndSum parsedArray =
        // It does not change any state and simply transforms the data provided as input to a valid output value
        // It does not perform any I/O (in this case printing out the result)
        // Now we can test the function just by checking that it provides the correct output value when given a specific input

        // if program argument array contains 3 elements (each of which are tuples)
        match parsedArray with
        |[|_, x;_, y;_, z|] 
            when checkAllAreValidInts parsedArray->
                // add together each of the integer values
                Ok(x+y+z)
        |[|x;y;z|] ->
                // ERROR: One or more arguments were invalid
                Error(ArgumentNotValidInt)
        |_-> 
            // ERROR: Invalid amount of arguments, expected 3 arguments but got <length of array>
            Error(InvalidArgumentAmount(Array.length parsedArray))

    // This is a PURE function
    let parseInputArgumentsToInts argv =
        // Convert array of strings to array of tuples which are the results
        // of attempting to parse each of the elements in the array
        Array.map Int32.TryParse argv

    // The main function is IMPURE because it uses a mixture of PURE and IMPURE functions
    [<EntryPoint>]
    let main argv = 
        // Take argv, 
        // try to parse the input arguments to ints, 
        // then check the input arguments and sum them together
        // print out the result aswell as returning the appropriate exit code
        argv |> parseInputArgumentsToInts |> checkInputAndSum |> printOutputAndGetExitCode


    // Our program may now be longer, but because we have broken it apart into smaller parts
    // which are explicitly either PURE or IMPURE functions, it is more easily testable!
    // we can just check that each PURE function provides the correct output value when given a specific input value
    // we have also introduced some custom types which give us the ability to return whether our
    // parsing of inputs was successful or there was some error
    // we can test these returned result values just by providing a given input and checking
    // the correct value of the result type is given
    // e.g.
    // checkInputAndSum [|true, 1; true, 2; true, 3|] = Ok(6)
    // checkInputAndSum [||] = Error(InvalidArgumentAmount(0))

    // The 0 here in the second tuple is what happens when we try to parse a string that is not a valid int
    // checkInputAndSum [|true, 1;false, 0; true, 3|] = Error(ArgumentNotValidInt) 

    // Notice that we could improve our error messaging by telling the user WHICH arguments are invalid
    // Let's now rewrite the program (with some minor modifications) to allow us to say which arguments are invalid
    // Fifth version (this provides improved error messaging telling us which arguments are invalid)

    open System

    // Creating a custom type to represent the different error messages
    // ErrorMessages can either be 'InvalidArgumentAmount (and the amount of arguments)
    // or 'ArgumentNotValidInt'
    type ErrorMessage =
        |InvalidArgumentAmount of int // How many arguments were provided?
        |ArgumentNotValidInt of int [] // Are one or more arguments invalid ints? Tell us which arguments they are?


    // Creating a custom type to represent whether the input parsing of the arguments was valid or invalid
    // This custom type uses type variables 
    // which allow F# to work out which types 'TSuc and 'TMsg are automatically
    type Result<'TSuc, 'TMsg> = 
        | Ok of 'TSuc // The result is Ok (return the successful result)
        | Error of 'TMsg // The result is an error (return an error value)

    // This is a PURE function
    let checkAllAreValidInts args =
        // Check that the first element in each TryParse result
        // within the array is true (and thus each conversion attempt was successful)
        Array.forall (fun (validInt,intValue) -> validInt) args

    // This is an IMPURE function
    let printOutputAndGetExitCode result =
        // Handle the output correctly (based on the specific 'Result' value provided
        match result with
        |Ok(summedValue) ->
            printfn "%i" summedValue
            0
        |Error(ArgumentNotValidInt(indices)) ->
            String.Join (", ", indices) // Join all the indices together into a string, separated with commas
            |> printfn "ERROR: Argument(s): %s were invalid, each argument should be a valid integer"
            1
        |Error(InvalidArgumentAmount(argumentAmount)) ->
            printfn "ERROR: Invalid amount of arguments, expected 3 arguments but got %i" argumentAmount
            1

    // This is a PURE function
    let getInvalidArgumentIndices args =
        // This function allows us to retrieve the indices of those arguments which are not valid integers
        Array.indexed args 
        |> Array.filter (fun(_, (isValid, _)) -> isValid=false) 
        |> Array.map (fun (idx,_) -> idx)

    // This is a PURE function
    let checkInputAndSum parsedArray =
        // It does not change any state and simply transforms the data provided as input to a valid output value
        // It does not perform any I/O (in this case printing out the result)
        // Now we can test the function just by checking that it provides the correct output value when given a specific input

        // if program argument array contains 3 elements (each of which are tuples)
        match parsedArray with
        |[|_, x;_, y;_, z|] 
            when checkAllAreValidInts parsedArray ->
                Ok(x+y+z)
        |[|x;y;z|] ->
                Error(ArgumentNotValidInt(getInvalidArgumentIndices parsedArray))
        |_ ->   Error(InvalidArgumentAmount(Array.length parsedArray))

    // This is a PURE function
    let parseInputArgumentsToInts argv =
        // Convert array of strings to array of tuples which are the results
        // of attempting to parse each of the elements in the array
        Array.map Int32.TryParse argv

    // The main function is IMPURE because it uses a mixture of PURE and IMPURE functions
    [<EntryPoint>]
    let main argv = 
        // Take argv, 
        // try to parse the input arguments to ints, 
        // then check the input arguments and sum them together
        // print out the result aswell as returning the appropriate exit code
        argv |> parseInputArgumentsToInts |> checkInputAndSum |> printOutputAndGetExitCode

    // We have now improved our error messaging a bit more
    // giving more useful messages to the user when they provide one or more invalid inputs
    // we can make some of our function definitions shorter by rewriting them in a "point free" style
    // this means that, when we are building a function up from a pipeline or composition of other functions
    // we don't have to specify the name of the argument to the function 
    // (because the first function in our pipeline/composition knows what the input needs to be already)
    // Sixth version (making our functions point free)
    open System

    // Creating a custom type to represent the different error messages
    // ErrorMessages can either be 'InvalidArgumentAmount (and the amount of arguments)
    // or 'ArgumentNotValidInt'
    type ErrorMessage =
        |InvalidArgumentAmount of int // How many arguments were provided?
        |ArgumentNotValidInt of int [] // Are one or more arguments invalid ints? Tell us which arguments they are?


    // Creating a custom type to represent whether the input parsing of the arguments was valid or invalid
    // This custom type uses type variables 
    // which allow F# to work out which types 'TSuc and 'TMsg are automatically
    type Result<'TSuc, 'TMsg> = 
        | Ok of 'TSuc // The result is Ok (return the successful result)
        | Error of 'TMsg // The result is an error (return an error value)

    // This is a PURE function
    // This function is also POINT FREE
    let checkAllAreValidInts =
        // Check that the first element in each TryParse result
        // within the array is true (and thus each conversion attempt was successful)
        Array.forall (fun (validInt,intValue) -> validInt)

    // This is an IMPURE function
    let printOutputAndGetExitCode result =
        // Handle the output correctly (based on the specific 'Result' value provided
        match result with
        |Ok(summedValue) ->
            printfn "%i" summedValue
            0
        |Error(ArgumentNotValidInt(indices)) ->
            String.Join (", ", indices) // Join all the indices together into a string, separated with commas
            |> printfn "ERROR: Argument(s): %s were invalid, each argument should be a valid integer"
            1
        |Error(InvalidArgumentAmount(argumentAmount)) ->
            printfn "ERROR: Invalid amount of arguments, expected 3 arguments but got %i" argumentAmount
            1

    // This is a PURE function
    // This function is also POINT FREE
    let getInvalidArgumentIndices =
        // This function allows us to retrieve the indices of those arguments which are not valid integers
        Array.indexed
        >> Array.filter (fun(_, (isValid, _)) -> isValid=false) 
        >> Array.map (fun (idx,_) -> idx)

    // This is a PURE function
    let checkInputAndSum parsedArray =
        // It does not change any state and simply transforms the data provided as input to a valid output value
        // It does not perform any I/O (in this case printing out the result)
        // Now we can test the function just by checking that it provides the correct output value when given a specific input

        // if program argument array contains 3 elements (each of which are tuples)
        match parsedArray with
        |[|_, x;_, y;_, z|] 
            when checkAllAreValidInts parsedArray ->
                Ok(x+y+z)
        |[|x;y;z|] ->
                Error(ArgumentNotValidInt(getInvalidArgumentIndices parsedArray))
        |_ ->   Error(InvalidArgumentAmount(Array.length parsedArray))

    // This is a PURE function
    // This function is also POINT FREE
    let parseInputArgumentsToInts =
        // Convert array of strings to array of tuples which are the results
        // of attempting to parse each of the elements in the array
        Array.map Int32.TryParse

    // The main function is IMPURE because it uses a mixture of PURE and IMPURE functions
    [<EntryPoint>]
    let main argv = 
        // Take argv, 
        // try to parse the input arguments to ints, 
        // then check the input arguments and sum them together
        // print out the result aswell as returning the appropriate exit code
        argv |> parseInputArgumentsToInts |> checkInputAndSum |> printOutputAndGetExitCode

    // But wait! We can also make our 'main' function POINT FREE aswell!
    // Seventh version (making the main function point free)
    open System

    // Creating a custom type to represent the different error messages
    // ErrorMessages can either be 'InvalidArgumentAmount (and the amount of arguments)
    // or 'ArgumentNotValidInt'
    type ErrorMessage =
        |InvalidArgumentAmount of int // How many arguments were provided?
        |ArgumentNotValidInt of int [] // Are one or more arguments invalid ints? Tell us which arguments they are?


    // Creating a custom type to represent whether the input parsing of the arguments was valid or invalid
    // This custom type uses type variables 
    // which allow F# to work out which types 'TSuc and 'TMsg are automatically
    type Result<'TSuc, 'TMsg> = 
        | Ok of 'TSuc // The result is Ok (return the successful result)
        | Error of 'TMsg // The result is an error (return an error value)

    // This is a PURE function
    // This function is also POINT FREE
    let checkAllAreValidInts =
        // Check that the first element in each TryParse result
        // within the array is true (and thus each conversion attempt was successful)
        Array.forall (fun (validInt,intValue) -> validInt)

    // This is an IMPURE function
    let printOutputAndGetExitCode result =
        // Handle the output correctly (based on the specific 'Result' value provided
        match result with
        |Ok(summedValue) ->
            printfn "%i" summedValue
            0
        |Error(ArgumentNotValidInt(indices)) ->
            String.Join (", ", indices) // Join all the indices together into a string, separated with commas
            |> printfn "ERROR: Argument(s): %s were invalid, each argument should be a valid integer"
            1
        |Error(InvalidArgumentAmount(argumentAmount)) ->
            printfn "ERROR: Invalid amount of arguments, expected 3 arguments but got %i" argumentAmount
            1

    // This is a PURE function
    // This function is also POINT FREE
    let getInvalidArgumentIndices =
        // This function allows us to retrieve the indices of those arguments which are not valid integers
        Array.indexed
        >> Array.filter (fun(_, (isValid, _)) -> isValid=false) 
        >> Array.map (fun (idx,_) -> idx)

    // This is a PURE function
    let checkInputAndSum parsedArray =
        // It does not change any state and simply transforms the data provided as input to a valid output value
        // It does not perform any I/O (in this case printing out the result)
        // Now we can test the function just by checking that it provides the correct output value when given a specific input

        // if program argument array contains 3 elements (each of which are tuples)
        match parsedArray with
        |[|_, x;_, y;_, z|] 
            when checkAllAreValidInts parsedArray ->
                Ok(x+y+z)
        |[|x;y;z|] ->
                Error(ArgumentNotValidInt(getInvalidArgumentIndices parsedArray))
        |_ ->   Error(InvalidArgumentAmount(Array.length parsedArray))

    // This is a PURE function
    // This function is also POINT FREE
    let parseInputArgumentsToInts =
        // Convert array of strings to array of tuples which are the results
        // of attempting to parse each of the elements in the array
        Array.map Int32.TryParse

    // The main function is IMPURE because it uses a mixture of PURE and IMPURE functions
    // This function is also POINT FREE
    [<EntryPoint>]
    let main = 
        // Try to parse the input arguments to ints, 
        // then check the input arguments and possibly sum them together
        // print out the result as well as returning the appropriate exit code
        parseInputArgumentsToInts >> checkInputAndSum >> printOutputAndGetExitCode

    // Our source code is getting a little crowded with explanatory comments
    // Let's now remove those comments that just helped explain the changes we made to rewrite our code
    // But keep in those comments that help to understand little parts of our code that
    // benefit from a short explanation
    // Eighth version (removing some of the longer comments)
    open System

    // Represents the different error messages
    type ErrorMessage =
        |InvalidArgumentAmount of int // How many arguments were provided?
        |ArgumentNotValidInt of int [] // Are one or more arguments invalid ints? Tell us which arguments they are


    // Was the input parsing of the arguments valid or invalid?
    type Result<'TSuc, 'TMsg> = 
        | Ok of 'TSuc // The result is Ok (return the successful result)
        | Error of 'TMsg // The result is an error (return an error value)


    let checkAllAreValidInts =
        // Check that the first element in each TryParse result
        // within the array is true (and thus each conversion attempt was successful)
        Array.forall (fun (validInt,intValue) -> validInt)


    let printOutputAndGetExitCode result =
        // Handle the output correctly (based on the specific 'Result' value provided)
        match result with
        |Ok(summedValue) ->
            printfn "%i" summedValue
            0
        |Error(ArgumentNotValidInt(indices)) ->
            String.Join (", ", indices) // Join all the indices together into a string, separated with commas
            |> printfn "ERROR: Argument(s): %s were invalid, each argument should be a valid integer"
            1
        |Error(InvalidArgumentAmount(argumentAmount)) ->
            printfn "ERROR: Invalid amount of arguments, expected 3 arguments but got %i" argumentAmount
            1


    let getInvalidArgumentIndices =
        // This function allows us to retrieve the indices of those arguments which are not valid integers
        Array.indexed
        >> Array.filter (fun(_, (isValid, _)) -> isValid=false) 
        >> Array.map (fun (idx,_) -> idx)


    let checkInputAndSum parsedArray =
        // if program argument array contains 3 elements (each of which are tuples)
        match parsedArray with
        |[|_, x;_, y;_, z|] 
            when checkAllAreValidInts parsedArray ->
                Ok(x+y+z)
        |[|x;y;z|] ->
                Error(ArgumentNotValidInt(getInvalidArgumentIndices parsedArray))
        |_ ->   Error(InvalidArgumentAmount(Array.length parsedArray))


    let parseInputArgumentsToInts =
        // Convert array of strings to array of tuples which are the results
        // of attempting to parse each of the elements in the array
        Array.map Int32.TryParse


    [<EntryPoint>]
    let main = 
        // Try to parse the input arguments to ints, 
        // then check the input arguments and possibly sum them together
        // print out the result as well as returning the appropriate exit code
        parseInputArgumentsToInts >> checkInputAndSum >> printOutputAndGetExitCode



*)



open System

// Represents the different error messages
type ErrorMessage =
    |InvalidArgumentAmount of int // How many arguments were provided?
    |ArgumentNotValidInt of int [] // Are one or more arguments invalid ints? Tell us which arguments they are


// Was the input parsing of the arguments valid or invalid?
type Result<'TSuc, 'TMsg> = 
    | Ok of 'TSuc // The result is Ok (return the successful result)
    | Error of 'TMsg // The result is an error (return an error value)


let checkAllAreValidInts =
    // Check that the first element in each TryParse result
    // within the array is true (and thus each conversion attempt was successful)
    Array.forall (fun (validInt,intValue) -> validInt)


let printOutputAndGetExitCode result =
    // Handle the output correctly (based on the specific 'Result' value provided)
    match result with
    |Ok(summedValue) ->
        printfn "%i" summedValue
        0
    |Error(ArgumentNotValidInt(indices)) ->
        String.Join (", ", indices) // Join all the indices together into a string, separated with commas
        |> printfn "ERROR: Argument(s): %s were invalid, each argument should be a valid integer"
        1
    |Error(InvalidArgumentAmount(argumentAmount)) ->
        printfn "ERROR: Invalid amount of arguments, expected 3 arguments but got %i" argumentAmount
        1


let getInvalidArgumentIndices =
    // This function allows us to retrieve the indices of those arguments which are not valid integers
    Array.indexed
    >> Array.filter (fun(_, (isValid, _)) -> isValid=false) 
    >> Array.map (fun (idx,_) -> idx)


let checkInputAndSum parsedArray =
    // if program argument array contains 3 elements (each of which are tuples)
    match parsedArray with
    |[|_, x;_, y;_, z|] 
        when checkAllAreValidInts parsedArray ->
            Ok(x+y+z)
    |[|x;y;z|] ->
            Error(ArgumentNotValidInt(getInvalidArgumentIndices parsedArray))
    |_ ->   Error(InvalidArgumentAmount(Array.length parsedArray))


let parseInputArgumentsToInts =
    // Convert array of strings to array of tuples which are the results
    // of attempting to parse each of the elements in the array
    Array.map Int32.TryParse


[<EntryPoint>]
let main = 
    // Try to parse the input arguments to ints, 
    // then check the input arguments and possibly sum them together
    // print out the result as well as returning the appropriate exit code
    parseInputArgumentsToInts >> checkInputAndSum >> printOutputAndGetExitCode