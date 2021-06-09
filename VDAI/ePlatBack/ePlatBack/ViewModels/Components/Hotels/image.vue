<style>
    input[type="file"] {
        position: absolute;
        top: -500px;
    }

    div.file-listing {
        width: 200px;
    }

    span.remove-file {
        color: red;
        cursor: pointer;
        float: right;
    }
</style>

<template>
    <div class="container">
        <div class="large-12 medium-12 small-12 cell">
            <label>
                Files
                <input type="file" @change="upload($event)" multiple id="file-input">
            </label>
        </div>
        <div class="large-12 medium-12 small-12 cell">
            <div v-for="(file, key) in files" class="file-listing">{{ file.name }} <span class="remove-file" v-on:click="removeFile( key )">Remove</span></div>
        </div>
        <br>
        <div class="large-12 medium-12 small-12 cell">
            <button v-on:click="addFiles()">Add Files</button>
        </div>
        <br>
        <div class="large-12 medium-12 small-12 cell">
            <button v-on:click="submitFiles()">Submit</button>
        </div>
    </div>
</template>

<script>  

    import axios from 'axios';
    export default {
        
    /*
      Defines the data used by the component
    */
    data(){
      return {
          files: '',
          file:'',
      }
    },

    /*
      Defines the method used by the component
    */
        methods: {
            upload: function (event) {
                let files = new FormData();
               // let file = event.target.files[0];
                console.log(event.target.files.length);
                for (var i = 0; i < event.target.files.length; i++) {
                    files.append('file', event.target.files[i])
                }
              
                console.log(files.get('file'));
                let config = {
                    header: {
                        'Content-Type': 'multipart/form-data'
                    }
                }

                axios.post('/Content/management/uploadImage', {

                    files: files,
                    id: 62
                }, config).then(
                    response => {
                        console.log("El resutlado es: "+response.data);

                    }
                )
            },
      /*
        Adds a file
      */
      addFiles(){
        this.$refs.files.click();
      },

      submitFiles(){
        
        let formData = new FormData();

       
          formData.append('file', this.files);
          

          for (var pair of formData.entries()) {
              console.log(pair[0] + ', ' + pair[1]);
          }
          console.log(formData.get('file'));
       
          
          var headers = {
              'Content-Type': 'multipart/form-data'
          };
          
         




          $.ajax({
              url: "/Content/management/uploadImage",
              type: "POST",
              dataType: "JSON",
              data: formData,
              contentType: false,
              processData: false,
              success: function (result) {
                  console.log("resultado "+result);
              }
          })









        },





      /*
        Handles the uploading of files
      */
      handleFilesUpload(){
        let uploadedFiles = this.$refs.files.files;

        /*
          Adds the uploaded file to the files array
        */
        for( var i = 0; i < uploadedFiles.length; i++ ){
          this.files.push( uploadedFiles[i] );
          }

          this.files = this.$refs.files.files;
      },

      /*
        Removes a select file the user has uploaded
      */
      removeFile( key ){
        this.files.splice( key, 1 );
      }
    }
  }
</script>