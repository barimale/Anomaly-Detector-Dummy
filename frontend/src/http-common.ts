import axios from "axios";

export default axios.create({
  baseURL: "http://localhost:53654",
  headers: {
    "Content-type": "application/json",
  },
});
