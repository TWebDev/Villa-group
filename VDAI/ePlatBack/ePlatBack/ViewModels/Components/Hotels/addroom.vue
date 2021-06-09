<template>

    <div v-if="destroyroom == false">

        <div v-if="newroom">

            {{update}}
            {{roomid}}
        </div>



        <div class="row">
            <div class="col-sm-4">
                <span>Room Name</span> <input type="text" class="form-control m-1" v-model="roomname" name="other" value="" />
            </div>
            <div class="col-sm-4">
                <span>Price</span> <input type="text" class="form-control m-1" v-model="price" name="other" value="" />
            </div>

            <div class="col-sm-12">
                <ckeditor :editor="editor" v-model="description"></ckeditor>

            </div>
        </div>
     
        <div class="col-sm-6 col-md-4 col-lg-4 mt-2">
            <b-card v-if="roompic !== ''"
                    :img-src="'/Images/senses/'+ roompic"
                    img-alt="Image"
                    img-top
                    tag="article"
                    style="max-width: 30rem;"
                    class="mb-2">

                <b-button @click="removePictureRoom" variant="primary"> <i class="material-icons md-48" style="vertical-align: middle;"> delete</i></b-button>
            </b-card>
            <div class="custom-file mb-3" v-if="roompic == '' ">
                <input type="file" ref="files" hidden class="custom-file-input" :id="id" @change="uploadRoom($event)">
                <label class="custom-file-label" :for="id">Seleccionar Archivo</label>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-3" v-for="(url, key) in roomurls" v-if="roomurl">

                <b-card :img-src="url"
                        img-alt="Image"
                        img-top
                        tag="article"
                        style="max-width: 30rem;"
                        class="mb-2">

                    <b-button @click="removePictureRoom(key,'new')" variant="danger"> <i class="material-icons md-48" style="vertical-align: middle;"> delete</i></b-button>
                </b-card>

            </div>
            <div class="col-sm-12 text-right">
                <button class="btn btn-danger" @click="deleteRoom">Delete Room</button>
            </div>
        </div>
        <hr />
    </div>

</template>


<script>
    import { bus } from './app.js';
    import axios from 'axios';
    import vSelect from 'vue-select';
    import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
    import CKEditor from '@ckeditor/ckeditor5-vue';




    export default {
        props: ['placeid', 'data', 'picturesrooms'],
        data: function () {
            return {
                countnewroom: 1,
                roomcount: 1,
                roomname: '',
                price: '',
                roomid: "",
                editor: ClassicEditor,
                squarefeet: '',
                error: '',
                destroyroom: false,
                urls: [],
                roomurls: [],
                roomurl: [],
                loading: true,
                main: '',
                latitude: '',
                type: 1,
                images: null,
                files: [],
                roomfiles: [],
                tempevent: [],
                roomtempevent: [],
                formdata: [],
                roomformdata: [],
                description: "",
                //*************
                id: '',
                removepic: false,
            }
        },
        components: {
            ckeditor: CKEditor.component,



        },
        methods: {
            getFields: function () {
                var date = new Date();
                var data = {
                    placeid: this.placeid,
                    sys: 7,
                    room: this.roomname,
                    date: date,
                    price: this.price,
                    description: this.description

                };

                return data;
            },
            removePictureRoom: function (key, idsys) {
                var r = this;
                if (idsys == "new") {
                    this.roomurls.splice(key, 1);
                    this.roomfiles.splice(key, 1);
                    this.roomtempevent = this.files;
                } else {

                    var r = this;
                    var text = "Do you want to delete this image ?";
                    $.confirm({
                        title: 'Delete Image ',
                        content: text,
                        typeAnimated: true,
                        type: 'red',
                        buttons: {
                            delete: {
                                text: 'Delete',
                                btnClass: 'btn-red',
                                action: function () {
                                    r.typerequest = "Please wait a moment...";

                                    axios.post('/Content/management/deleteImage', {
                                        id: r.pictureid,
                                        idsys: r.picturesysid,

                                    }).then(response => {

                                        if (response.data == "ok") {
                                            $.alert('Deleted succesfully.');
                                            r.removepic = true;

                                        }
                                    });
                                }
                            },
                            Cancel: function () {
                            },
                        }
                    });

                }

            },

            uploadRoom: function (event) {
                // this.url = URL.createObjectURL(file);
                let uploadedFiles = this.$refs.files.files;

                /*
                  Adds the uploaded file to the files array
                */
                for (var i = 0; i < uploadedFiles.length; i++) {
                    this.roomfiles.push(uploadedFiles[i]);
                }
                for (var i = 0; i < event.target.files.length; i++) {

                    this.roomurls.push(URL.createObjectURL(event.target.files[i]));
                }
                this.roomtempevent = event.target.files;

            },
            deleteRoom: function () {
                var r = this;
                $.confirm({
                    title: 'Delete Room ' + r.roomname,
                    content: "Do you want to delete this room ?",
                    typeAnimated: true,
                    type: 'red',
                    buttons: {
                        delete: {
                            text: 'Delete',
                            btnClass: 'btn-red',
                            action: function () {
                                r.typerequest = "Please wait a moment...";
                                axios.post('/Content/management/DeleteRoom', {
                                    id: r.roomid,
                                    sysid: r.picturesysid,
                                    picid: r.pictureid,

                                }).then(response => {

                                    if (response.data == "ok") {
                                        $.alert('Deleted succesfully.');
                                        r.destroyroom = true;
                                        r.roomname = "";

                                    }
                                });
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });
            },
        },
        computed: {
            update: function () {
                if (this.data !== "new") {
                    this.price = this.data[0].price;
                    this.roomname = this.data[0].name;
                    this.description = this.data[0].description;
                    this.images = this.data[0].picture;
                    this.sysid = this.data[0].picsysid;
                    this.picid = this.data[0].picid;
                    this.roomid = this.data[0].roomid;


                    //  this.images = this.data[0].picture;

                }
            },
            roompic: function () {
                if (this.removepic == false) {
                    var r = this;
                    var img = "";
                    Array.from(this.picturesrooms).forEach(function (data) {
                        if (data.roomid == r.data[0].roomid) {
                            img = data.picture;
                        }

                    });

                    return img;
                } else {
                    return "";
                }


            },
            pictureid: function () {
                var r = this;
                var picid = "";
                Array.from(this.picturesrooms).forEach(function (data) {
                    if (data.roomid == r.data[0].roomid) {
                        picid = data.picid;
                    }

                });
                return picid;
            },
            picturesysid: function () {
                var r = this;
                var picid = "";
                Array.from(this.picturesrooms).forEach(function (data) {
                    if (data.roomid == r.data[0].roomid) {
                        picid = data.picsysid;
                    }

                });
                return picid;
            },
            newroom: function () {
                if (this.data == "new") {
                    return false;
                } else {
                    return true;
                }

            }

        },

        mounted() {

            this.id = Math.random();



            bus.$on('newroom', obj => {
               
                this.countnewroom = this.countnewroom + 1;
                console.log("count es " + this.countnewroom);
                if (this.countnewroom < 3) {
                
                    if (this.destroyroom == false) {

                        var url = "";
                        var data = this.getFields();
                        let files = new FormData();
                        console.log("ROOM ID ANTES DE ELEGIR LA URL " + this.roomid);
                        if (this.roomid == "") {
                            url = "/Content/management/AddRoom";
                            data.placeid = this.placeid;
                        } else {
                            url = "/Content/management/EditRoom";
                            data.placeid = this.roomid;
                           
                        }
                     

                        for (var i = 0; i < this.roomfiles.length; i++) {
                            files.append('file', this.roomfiles[i])

                        }

                       

                       

                        if (this.roomname !== "") {

                            if (obj.id == "") {
                               
                            } else {
                                data.placeid = obj.id;
                            }
                            axios.post(url, {
                                placeid: data.placeid,
                                sys: data.sys,
                                room: data.room,
                                date: data.date,
                                price: data.price,
                                description: data.description,
                            }).then(response => {
                                if (response.data == "not") {

                                } else {


                                    this.roomid = response.data;
                                    if (url == "/Content/management/AddRoom") {
                                        files.append('id', response.data);

                                    } else {
                                        files.append('id', data.placeid);
                                    }
                                    files.append('sys', 7);
                                    this.roomformdata = files;

                                    let config = {
                                        header: {
                                            'Content-Type': 'multipart/form-data'
                                        }
                                    }
                                    if (this.roomfiles.length > 0) {
                                        axios.post('/Content/management/uploadImage', this.roomformdata, config).then(
                                            response => {

                                                if (response.data == "ok") {

                                                    /*  axios.post('/Content/management/updatePLaceID', {
                                                          id: this.roomid,
                                                          sys: 7
              
                                                      }).then(
                                                          response => {
              
                                                              console.log("actualizado las imagenes" + response.data);
                                                              axios.post('/Content/management/getImagesPlace', {
                                                                  id: this.roomid,
                                                                  sys: 7,
                                                              }).then(
                                                                  response => {
                                                                      this.images = [];
                                                                      this.images = response.data;
                                                                      this.roomfiles = [];
                                                                      this.roomtempevent = [];
                                                                      this.roomformdata = [];
                                                                      this.roomurls = [];
                                                                  
              
                                                                  });
                                                          }
                                                      );*/
                                                }
                                            }

                                        );
                                    }


                                    bus.$emit('updaterooms', {
                                    });

                                }


                            });

                        }


                }
                }
                });
        

            },
        }

</script>
