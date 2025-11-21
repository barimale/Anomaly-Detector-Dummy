import http from "../http-common";
import { Guid } from "guid-typescript";

const upload = (file: File, onUploadProgress: (progressEvent: any) => void): Promise<any> => {
  let formData = new FormData();

  formData.append("uploadedFile", file);
  var id = Guid.create().toString();

  return http.post("/api/stream", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
      "Accept": "*/*",
      "User-Agent": "react-app",
      "X-SessionId": id
    },
    onUploadProgress,
  }).then((value) => {
    http.post("/api/notify", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
      "Accept": "*/*",
      "User-Agent": "react-app",
      "X-SessionId": id
    },
  })})
};

const getFiles = () : Promise<any> => {
  return http.get("/api/data/get");
};

const FileUploadService = {
  upload,
  getFiles,
};

export default FileUploadService;
