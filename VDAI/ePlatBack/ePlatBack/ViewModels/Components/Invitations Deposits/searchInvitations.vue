    <template>
        <div>
            <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pb-2 border-bottom">
                <h1 class="h2">Invitations Deposits</h1>
                <div class="btn-toolbar mb-2 mb-md-0">
                    <button type="button" class="btn btn-sm mr-2">
                        <i class="material-icons">search</i>
                    </button>
                </div>
            </div>
            <div class="card mb-2 search-card">
                <div class="card-body">
                    <div class="form-row">
                        <div class=" col-md-3 col-lg-2">
                            <span>From Date</span>
                            <input class="form-control" type="text">
                        </div>
                        <div class=" col-md-3 col-lg-2">
                            <span>To Date</span>
                            <input class="form-control" type="text">
                        </div>
                        <div class=" col-md-3 col-lg-2">
                            <span>Presentation Place</span>
                            <input class="form-control" type="text">
                        </div>
                        <div class=" col-md-3 col-lg-2">
                            <span>Shift</span>
                            <select class="form-control">
                                <option>option 1</option>
                            </select>
                        </div>
                        <div class=" col-md-3 col-lg-2">
                            <span>Program</span>
                            <select class="form-control">
                                <option>option 1</option>
                            </select>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card mb-2">
                <div class="card-body">
                    <deposits></deposits>
                </div>
            </div>
        </div>
    </template>
    <script>
        import { bus } from './app.js';
        import axios from 'axios';
        import invitationDepostis from './deposits.vue';
        export default ({
            components: {
                'deposits': invitationDepostis
            },
            data:function() {
                return {
                    fromDate: '',
                    toDate: '',     
                    presentationPlaceID: null,
                    programID:null
                }
            },
            methods: {

            }
        })
    </script>
    <style>

    </style>