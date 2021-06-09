<template>
    <div class="mb-5">
        <div v-if="loading" class="text-center">
            <span>{{typerequest}}</span> 
            <pulse-loader color="#31A3DD"></pulse-loader>
        </div>
        <div v-else>

            <div class="text-center">
                <i class="fa fa-pencil-square-o fa-2x" aria-hidden="true"></i> <h3 style="position: relative"> {{title}} Details</h3>
            </div>


            <!-- Formulario del hotel -->

            <div class="container">
                <div class="col-sm-12">
                    <div class="row">
                        <div class="col-sm-3"></div>
                        <div class="col-sm-6 card m-2">
                            <div class="card-body text-center">
                                <div class="card-title">
                                    <h5>Orientation</h5>
                                </div>
                                <b-form-group>
                                    <b-form-checkbox-group v-model="selectedori" :options="orientations">
                                    </b-form-checkbox-group>
                                </b-form-group>
                                <span style="color: red"><strong>{{errororientation}}</strong></span>
                            </div>
                        </div>
                        <div class="col-sm-3"></div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 mt-2 card  ">
                            <div class="card-title mt-2">
                                <div class="col-sm-12">
                                    <div class="row">
                                        <div class=" col-sm-6 text-left">
                                            <h5>General Info</h5>
                                        </div>
                                        <div class="col-sm-6 text-right">
                                            <h5>Date Created </h5> {{fecha(datecreated)}}
                                        </div>
                                        <div class="col-sm-12 text-right">
                                            <a :href="'https://www.sensesofmexico.com/#/preview/magazine/id/'+articleid" target="_blank"><button class="btn btn-primary mt-3">Preview</button></a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row m-3 card-body">
                                <div class="col-sm-8 col-md-8 col-lg-8  mb-2">
                                    <span>Title</span>
                                    <input id="title" @focus="maxLenght" class="form-control" type="text" v-model="title" @blur="buildUrl">
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4  mb-2">
                                    <span>Status</span>
                                    <v-select v-model="status" :options="activeinactive"></v-select>
                                </div>
                                <div class="col-sm-12 mb-2">
                                    <span>Short Content</span>
                                    <textarea class="form-control" type="text" v-model="shortcontent"></textarea>
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4  mb-2">
                                    <span>Structure</span>
                                    <v-select v-model="structure" :options="structures"></v-select>
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4 mb-2">
                                    <span>Type of Article</span>
                                    <v-select v-model="pagetype" :options="pagestypes"></v-select>
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4 mb-2">
                                    <span>Author</span>
                                    <v-select v-model="copywriter" :options="copywriters"></v-select>
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4 mb-2">
                                    <span>Publication Date</span>
                                    <div class="form-group">
                                        <div class="input-group date" id="datetimepicker" data-target-input="nearest">
                                            <input type="text" prefix="calendar" v-on:blur="calendarInput" id="calendario" class="form-control datetimepicker-input" data-target="#datetimepicker" :value="today" />
                                            <div class="input-group-append" data-target="#datetimepicker" data-toggle="datetimepicker">
                                                <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4 col-md-4 col-lg-4 mb-2 mt-4">
                                    <b-form-checkbox id="checkbox1"
                                                     v-model="showinmenu">
                                        Show in Home Page
                                    </b-form-checkbox>
                                </div>

                                <div class="col-sm-12  mb-2">
                                    <span style="color: red"><strong>{{error}}</strong></span>
                                </div>
                            </div>
                            </div>
                    </div>
                    <div class="row mt-2">
                        <div class="card col-sm-12">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-sm-6 col-md-6 col-lg-6 mt-4">
                                        <div>
                                            <h5>Title Seo</h5>
                                        </div>
                                        <div class="input-group">
                                            <input type="text" class="form-control" id="inlineFormInputGroup" v-model="titleseo">
                                        </div>
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-6 mt-4">
                                        <div>
                                            <h5>Friendly URL</h5>
                                        </div>
                                        <div class="input-group">
                                            <div class="input-group">
                                                <input type="text" class="form-control" id="inlineFormInputGroup" v-model="friendlyurl">
                                            </div>

                                        </div>
                                    </div>
                                    <div class="col-sm-12">
                                        <span style="color: red"><strong>{{errorseo}}</strong></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 mt-2 card  ">
                            <div class="row mb-2 card-body">
                                <div class=" col-sm-12">
                                    <div class="">
                                        <div class="">
                                            <h5>Title With Format</h5>
                                        </div>
                                        <div class="mb-3">
                                            <ckeditor :editor="editor"  @ready="onReady" v-model="titleformat"></ckeditor>
                                        </div>
                                        <div class="">
                                            <h5>Content</h5>
                                        </div>
                                        <div>
                                            <ckeditor :editor="editor"  @ready="onReady" v-model="content" ></ckeditor>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mt-3">

                        <div class="col-sm-12 col-md-6 col-lg-4" v-for="(url, key) in images">

                            <b-card :img-src="'/Images/senses/'+ url.name+'?width=300&height=200&mode=crop&quality=100&autorotate=true'"
                                    img-alt="Image"
                                    img-top
                                    tag="article"
                                    style="max-width: 30rem; "
                                    class="mb-2">
                                <b-button @click="removePicture(key, url.idsys, url.id)" variant="danger"> <i class="material-icons md-48" style="vertical-align: middle;"> delete</i></b-button>

                                <span v-if="url.main == false">

                                    <b-button @click="selectMain(url)" variant="outline-primary"> Select as main</b-button>
                                </span>
                                <span v-else class="text-right">

                                    <b-button @click="selectMain(url)" variant="success"> Main</b-button>
                                </span>

                            </b-card>

                        </div>

                    </div>

                    <div class="row">
                        <div class="col-sm-12">
                            <hr />

                            <div class="col-sm-6 col-md-4 col-lg-4">
                                <div class="custom-file mb-3">
                                    <input type="file" ref="files" class="custom-file-input" id="customFileLang" multiple @change="upload($event)">
                                    <label class="custom-file-label" for="customFileLang">Seleccionar Archivo</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-3" v-for="(url, key) in urls" v-if="url">


                                    <b-card :img-src="url"
                                            img-alt="Image"
                                            img-top
                                            tag="article"
                                            style="max-width: 30rem;"
                                            class="mb-2">

                                        <b-button @click="removePicture(key,'new')" variant="primary"> <i class="material-icons md-48" style="vertical-align: middle;"> delete</i></b-button>
                                    </b-card>

                                </div>
                            </div>

                        </div>
                    </div>

                    <div class="row mt-2">
                        <div class="col-sm-12">
                            <div class="text-right">
                                <button class="btn btn-primary" @click="saveArticle">Save</button>
                                <button class="btn btn-danger" @click="deleteArticle"><i class="material-icons md-48" style="vertical-align: middle;"> delete</i></button>
                            </div>

                        </div>
                    </div>

                </div>
            </div>
        </div>
        <!-- ********************* -->
    </div>




</template>

<script>



    import { bus } from './app.js';
   
    import DecoupledEditor from '@ckeditor/ckeditor5-build-decoupled-document';
    import CKEditor from '@ckeditor/ckeditor5-vue';

    import axios from 'axios';
    import vSelect from 'vue-select';
    import PulseLoader from 'vue-spinner/src/PulseLoader.vue'
    Vue.component('v-select', vSelect)
    var arreglo = [];
    export default {
        props: ['id'],
            data: function () {
                return {
                    error: '',
                    title: '',
                    counter: 1,
                    today:'',
                    copywriters: [],
                    copywriter: [],
                    showinmenu: false,
                    loading: true,
                    pagestypes: [],
                    pagetype:[],
                    editor: DecoupledEditor ,
                    perPage: 5,
                    currentPage: 1,
                    countrows: 2,
                    errorseo: '',
                    titleseo: '',
                    friendlyurl:'',
                    staticurl: '/magazine/',
                    url: '',
                    slider: [],
                    sliders: [
                        {
                            value: true,
                            label: "Show"
                        },
                        {
                            value: false,
                            label: "No Show"
                        }
                    ],
                    // campos del hotel
                    hotelname: '',
                    urls: [],
                    errororientation: '',
                    files: [],
                    titleformat:'',
                    type: 1,
                    orientations: [],
                    orientation: '',
                    selectedori: [],
                    content: '',
                    images: [],
                    typerequest: '',
                    formdata: [],
                    tempevent: '',
                    status: null,
                    response: "response",
                    structures: [],
                    main: '',
                    articleid:'',
                    structure: [],
                    shortcontent: '',
                    publicationdate:'',
                    activeinactive: [
                        {
                            value: 1,
                            label: "Active"
                        },
                        {
                            value: 0,
                            label: "Inactive"
                        }
                    ],
                    
                    //*************


                }
        },
        components: {
            'PulseLoader': PulseLoader,
            ckeditor: CKEditor.component
        },
        computed: {
            check: function () {
                var c = this.counter;
                axios.post('/Content/management/getImagesPlace', {
                    id: this.articleid,
                    sys: 9,
                }).then(
                    response => {
                        this.images = [];
                        this.images = response.data;
                        console.log(response.data);
                    });
                return this.counter;
            },
            dateclean: function () {

            },
        },
        methods: {
            maxLenght: function () {
                document.getElementById("title").maxLength = "90";
            },
            datePublished: function (d) {
                if (d == null) {
                    return "";
                } else {
                    var getdate = d.split("/Date(").join("");
                    getdate = getdate.split(")/").join("");
                    console.log("fecha " + getdate);
                    var date = new Date(parseInt(getdate));

                    var dia = date.getUTCDate();
                    var mes = date.getUTCMonth();
                    var anio = date.getUTCFullYear();
                    var fecha = new Date(anio, mes, dia);
                    
                    var month = fecha.getUTCMonth() + 1;
                    return month.toString().padStart(2, "0") + "/" + fecha.getUTCDate().toString().padStart(2, "0") + "/" + fecha.getUTCFullYear();
                }
              
            },
            calendarInput: function () {

                
                this.publicationdate = $('#calendario').val();
                console.log(this.publicationdate.substr(0, 10));
                var date = new Date(this.publicationdate.substr(0, 10));
                console.log(date);
                var dia = date.getUTCDate();
                var mes = date.getUTCMonth();
                var anio = date.getUTCFullYear();
                var fecha = new Date(anio, mes, dia);
                var month = fecha.getUTCMonth() + 1;
                var day = fecha.getUTCDate();
                this.today = month.toString().padStart(2, "0") + "/" + day.toString().padStart(2, "0") +"/"+ fecha.getUTCFullYear();
                this.$forceUpdate();


            },
            onReady(editor) {
                // Insert the toolbar before the editable area.
                editor.ui.view.editable.element.parentElement.insertBefore(
                    editor.ui.view.toolbar.element,
                    editor.ui.view.editable.element
                );
            },
            buildUrl: function (evento) {
                if (this.titleseo == "") {

                    var url = this.title.toLowerCase().split(' ').join('-');
                    url = url.toLowerCase();
                    url = url.replace(/---/g, '-');
                    url = url.replace(/--/g, '-');
                    url = url.replace(/ /g, '-');
                    url = url.replace(/\?/g, '');
                    url = url.replace(/!/g, '');
                    url = url.replace(/,/g, '');
                    url = url.replace(/á/g, 'a');
                    url = url.replace(/é/g, 'e');
                    url = url.replace(/í/g, 'i');
                    url = url.replace(/ó/g, 'o');
                    url = url.replace(/ú/g, 'u');
                    url = url.replace(/'/g, '');
                    url = url.replace(/"/g, '');
                    url = url.replace(/’/g, '');
                    url = url.replace(/”/g, '');
                    url = url.replace(/“/g, '');
                    url = url.replace(/&/g, 'and');
                    url = url.replace(/ñ/g, 'n');
                    url = url.replace(/=/g, '');
                    var urls = "/magazine/" + url;
                    this.friendlyurl = urls;
                    this.titleseo = this.title;
                }

            },
            fecha: function (fecha) {
                if (fecha !== undefined) {

                    fecha = fecha.split("/Date(").join("");
                    fecha = fecha.split(")/").join("");
                    var dia = new Date(parseInt(fecha)).getDate();
                    var mes = new Date(parseInt(fecha)).getMonth();
                    mes = mes + 1;
                    var año = new Date(parseInt(fecha)).getFullYear();
                    fecha = dia + "-" + mes + "-" + año;
                    return fecha;
                }

            },
            deleteArticle: function () {
                var date = new Date();
                var text = "Do you want to delete " + this.title + " article?";
                var r = this;
                $.confirm({
                    title: 'Delete Article ',
                    content: text,
                    typeAnimated: true,
                    type: 'red',
                    buttons: {
                        delete: {
                            text: 'Delete',
                            btnClass: 'btn-red',
                            action: function () {
                                r.typerequest = "Please wait a moment...";

                                axios.post('/Content/management/deleteArticle', {
                                    pageid: r.articleid,
                                    date: date,
                                    sys: 9
                                }).then(response => {

                                    if (response.data == "ok") {
                                      
                                        $.alert('Deleted succesfully.');
                                        r.typerequest = "";
                                        bus.$emit('updatearticle', {
                                            response: 'deleted',
                                          
                                        });
                                    }
                                });
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });

                
            },
            getData: function () {

                axios.get('/Content/management/getDataPlaces', {


                }).then(response => {
                    this.orientations = response.data[2][0];
                    this.structures = response.data[7][0];
                    this.copywriters = response.data[8][0];
                    this.pagestypes = response.data[9][0]

                 

                });
            },
            removePicture: function (key) {
                this.urls.splice(key, 1);
                this.files.splice(key, 1);
                this.tempevent = this.files;
            },
            upload: function (event) {

                // this.url = URL.createObjectURL(file);
                let uploadedFiles = this.$refs.files.files;

                /*
                  Adds the uploaded file to the files array
                */
                for (var i = 0; i < uploadedFiles.length; i++) {
                    this.files.push(uploadedFiles[i]);
                }
                for (var i = 0; i < event.target.files.length; i++) {

                    this.urls.push(URL.createObjectURL(event.target.files[i]));
                }
                this.tempevent = event.target.files;

            },

            getFields: function () {
            
                var date = new Date();
             
                var article = {
                    articleid: this.articleid,
                    title: this.title,
                    placetypeid: 1,
                    status: this.status.value,
                    content: this.content,
                    orientations: this.selectedori,
                    structure: this.structure.value,
                    date: date,
                    sysitem: 9,
                    staticurl: this.staticurl,
                    titleseo: this.titleseo,
                    friendlyurl: this.friendlyurl,
                    type: this.pagetype.value,
                    shortcontent: this.shortcontent,
                    titleformat: this.titleformat,
                    author: this.copywriter.value,
                    publicationdate: this.publicationdate,
                    showinmenu: this.showinmenu,

                };
             
                return article;
            },

            selectMain: function (data) {
                Array.from(this.images).forEach(function (data) {
                    data.main = false;
                });
                data.main = true;
                this.main = data.idsys;
            },
            verifyFields: function () {
                if (this.title.trim() == "" || this.status == "" || this.publicationdate == ""
                    || this.status == null || this.structure == "" || this.structure == null || this.copywriter == null) {
                    this.error = "Please complete all the require fileds.";

                    return true;
                } else {
                    this.error = "";
                    if (this.selectedori.length < 1) {
                        if (this.selectedori.length < 1) {
                            this.errororientation = "Please, select at least one orientation."
                            return true;
                        } else {
                            this.errororientation = ""
                        } "Please, select at least one orientation."
                        return true;
                    } else {
                        this.errororientation = ""
                    }
                    return false;
                }
            },
            removePicture: function (key, idsys, id) {
                if (idsys == "new") {
                    this.urls.splice(key, 1);
                    this.files.splice(key, 1);
                    this.tempevent = this.files;
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
                                        id: id,
                                        idsys: idsys,
                                        sys: 9
                                    }).then(response => {

                                        if (response.data == "ok") {
                                            r.images.splice(key, 1);
                                            $.alert('Deleted succesfully.');
                                            r.typerequest = "";

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
            saveArticle: function () {

                
                var error = this.verifyFields();
             
                if (!error) {
                    var article = this.getFields();
                } else {
                    return this;
                }

             
                var text = "Do you want to update " + article.title + " article?";
                 
                let files = new FormData();
                // let file = event.target.files[0];
               
                for (var i = 0; i < this.files.length; i++) {
                    files.append('file', this.files[i])

                }
                this.formdata = files;

                var data = JSON.stringify(article);
                var r = this;

                $.confirm({
                    title: 'Update Article ',
                    content: text,
                    typeAnimated: true,
                    type: 'blue',
                    buttons: {
                        save: {
                            text: 'Continue',
                            btnClass: 'btn-blue',
                            action: function () {
                                
                                r.loading = true;
                                r.typerequest = "Please wait a moment...";
                                axios.post('/Content/management/updateArticle', {
                                    data: data,
                                    sys: 9

                                }).then(response => {
                                    
                                    if (r.main !== "") {
                                        axios.post('/Content/management/updateMain', {
                                            id: r.articleid,
                                            idsys: r.main,
                                            sys: 9,
                                        }).then(
                                            response => {

                                            });
                                    }

                                    let config = {
                                        header: {
                                            'Content-Type': 'multipart/form-data'
                                        }
                                    }
                                    if (r.files.length > 0) {
                                        files.append('id', r.articleid);
                                        files.append('sys', 9)

                                        axios.post('/Content/management/uploadImage', r.formdata, config).then(
                                            response => {

                                                if (response.data == "ok") {
                                                    axios.post('/Content/management/getImagesPlace', {
                                                        id: r.articleid,
                                                        sys: 9,
                                                    }).then(
                                                        response => {
                                                            r.images = [];
                                                            r.images = response.data;
                                                            r.files = [];
                                                            r.tempevent = [];
                                                            r.formdata = [];
                                                            r.typerequest = "";
                                                            r.urls = [];
                                                            r.$refs.files.files.value = "";

                                                        });
                                                    /*
                                                    axios.post('/Content/management/updatePLaceID', {
                                                        id: r.placeid,
                                                        sys: 6
                                                     
                                                    }).then(
                                                        response => {

                                                            a
                                                               

                                                        }
                                                    );
                                                    */
                                                }
                                            }
                                        );
                                    }
                                    r.loading = false;
                                    $.alert('Updated succesfully.');
                                    r.typerequest = "";
                                   
                                    bus.$emit('updatearticle', {
                                        response: 'updated',
                                        articlename: r.title,
                                        status: r.status.value,

                                    });


                                    });


                               
                            }
                        },
                        Cancel: function () {
                        },
                    }
                });
                
                this.counter = this.counter + 1;
            },

            cleanFields: function () {
                this.title = "";
                this.selectedori = [];
                this.status = "";
                this.structure = "";
                this.content = "";



            },
            getDataSelected: function (id) {
                if (id == undefined) {
                    id = this.id;
                }
                this.images = [];
                this.articleid = id;
              
                axios.post('/Content/management/getItemSelected', {
                    id: id,
                    sys: 9

                }).then(
                    response => {
                        var ori = [];
                        if (response.data[1][0] !== null) {
                            Array.from(response.data[1][0]).forEach(function (data) {
                                ori.push(data.value);
                            });
                        }
                        this.selectedori = ori;
                        var article = response.data[0][0];
                        var seo = response.data[3][0];
                        this.images = response.data[2][0];
                        this.datecreated = article[0].date;
                        this.title = article[0].title;
                        this.structure = {
                            value: article[0].structureid,
                            label: article[0].structure

                        };
                        this.copywriter = {
                            value: article[0].authorid,
                            label: article[0].author
                        };
                        this.titleformat = article[0].titleformat;
                        this.today = this.datePublished(article[0].publicationdate);
                        this.publicationdate = this.today;
                        this.shortcontent = article[0].shortcontent;
                        this.showinmenu = article[0].showinmenu;
                        var label = 1;
                        if (article[0].status == true) {
                            this.status = {
                                value: 1,
                                label: "Active",
                            };
                        } else {
                            this.status = {
                                value: 0,
                                label: "Inactive",
                            };
                        }
                        this.pagetype = {
                            value: article[0].type,
                            label: article[0].typepage
                        };
                        this.titleseo = seo.title;
                        this.friendlyurl = seo.friendlyurl;
                        this.content = article[0].content;
                        this.loading = false;
                        
                        
                    }
                );
            },
            dateFormat: function () {
                $(function () {
                    $('#datetimepicker').datetimepicker({
                        format: 'DD-MM-YYYY'
                    });
                });
            },

        },

        mounted() {
            let editor;
            this.dateFormat();
            this.getData();
            this.getDataSelected();
            bus.$on('updatearticle', obj => {
                
                this.getDataSelected(obj.id);
                
                
            });
           
           // this.$refs.phone.maxLength = 4;
            },
        }

</script>

