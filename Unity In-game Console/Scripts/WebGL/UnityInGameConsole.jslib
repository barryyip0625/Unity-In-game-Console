mergeInto( LibraryManager.library,
{
    CopyLogMessage : function (str){
        if(navigator.clipboard){
			navigator.clipboard.writeText( str ).then( function() { }, function( err )
			{
			    console.error( "Couldn't copy text by using clipboard.writeText: ", err );
			});
		}
        else{
            var textArea = document.createElement( 'textarea' );
			textArea.value = str;

            document.body.appendChild( textArea );
            textArea.focus();
            textArea.select();

            try{
				document.execCommand( 'copy' );
			}
			catch( err ){
				console.error( "Couldn't copy text by using document.execCommand", err );
			}

			document.body.removeChild( textArea );
        }
    }
});