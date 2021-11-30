function executeWrite(base64Image) {

    return axios.post(HTTP_HOST+`DotWriter/execute`, `base64Image=${base64Image}`)
}
