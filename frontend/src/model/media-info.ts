/**
 * Generated by the protoc-gen-ts.  DO NOT EDIT!
 * compiler version: 3.21.10
 * source: media-info.proto
 * git: https://github.com/thesayyn/protoc-gen-ts */
import * as pb_1 from "google-protobuf";
export enum MediaType {
    IMAGE = 0,
    VIDEO = 1,
    HEIC = 2
}
export enum ImportStepReportSeverity {
    INFORMATION = 0,
    WARNING = 1,
    ERROR = 2
}
export class MediaInfo extends pb_1.Message {
    #one_of_decls: number[][] = [[14], [15], [16], [17], [18], [19], [20], [21], [22], [23], [24], [25], [27], [28]];
    constructor(data?: any[] | ({
        id?: string;
        thumbnailUrl?: string;
        mediaUrl?: string;
        fullPath?: string;
        fileName?: string;
        fileExtension?: string;
        mediaType?: MediaType;
        fileSizeKb?: number;
        dateTimeOriginal?: number;
        width?: number;
        height?: number;
        thumbnailWidth?: number;
        thumbnailHeight?: number;
        thumbnail?: Uint8Array;
    } & (({
        videoDurationSec?: number;
    }) | ({
        pictureMaker?: string;
    }) | ({
        tag?: string;
    }) | ({
        latitude?: number;
    }) | ({
        latitudePole?: string;
    }) | ({
        longitude?: number;
    }) | ({
        longitudeSide?: string;
    }) | ({
        country?: string;
    }) | ({
        region?: string;
    }) | ({
        locality?: string;
    }) | ({
        address?: string;
    }) | ({
        venue?: string;
    }) | ({
        isFavorite?: boolean;
    }) | ({
        albumName?: string;
    })))) {
        super();
        pb_1.Message.initialize(this, Array.isArray(data) ? data : [], 0, -1, [], this.#one_of_decls);
        if (!Array.isArray(data) && typeof data == "object") {
            if ("id" in data && data.id != undefined) {
                this.id = data.id;
            }
            if ("thumbnailUrl" in data && data.thumbnailUrl != undefined) {
                this.thumbnailUrl = data.thumbnailUrl;
            }
            if ("mediaUrl" in data && data.mediaUrl != undefined) {
                this.mediaUrl = data.mediaUrl;
            }
            if ("fullPath" in data && data.fullPath != undefined) {
                this.fullPath = data.fullPath;
            }
            if ("fileName" in data && data.fileName != undefined) {
                this.fileName = data.fileName;
            }
            if ("fileExtension" in data && data.fileExtension != undefined) {
                this.fileExtension = data.fileExtension;
            }
            if ("mediaType" in data && data.mediaType != undefined) {
                this.mediaType = data.mediaType;
            }
            if ("fileSizeKb" in data && data.fileSizeKb != undefined) {
                this.fileSizeKb = data.fileSizeKb;
            }
            if ("dateTimeOriginal" in data && data.dateTimeOriginal != undefined) {
                this.dateTimeOriginal = data.dateTimeOriginal;
            }
            if ("width" in data && data.width != undefined) {
                this.width = data.width;
            }
            if ("height" in data && data.height != undefined) {
                this.height = data.height;
            }
            if ("thumbnailWidth" in data && data.thumbnailWidth != undefined) {
                this.thumbnailWidth = data.thumbnailWidth;
            }
            if ("thumbnailHeight" in data && data.thumbnailHeight != undefined) {
                this.thumbnailHeight = data.thumbnailHeight;
            }
            if ("videoDurationSec" in data && data.videoDurationSec != undefined) {
                this.videoDurationSec = data.videoDurationSec;
            }
            if ("pictureMaker" in data && data.pictureMaker != undefined) {
                this.pictureMaker = data.pictureMaker;
            }
            if ("tag" in data && data.tag != undefined) {
                this.tag = data.tag;
            }
            if ("latitude" in data && data.latitude != undefined) {
                this.latitude = data.latitude;
            }
            if ("latitudePole" in data && data.latitudePole != undefined) {
                this.latitudePole = data.latitudePole;
            }
            if ("longitude" in data && data.longitude != undefined) {
                this.longitude = data.longitude;
            }
            if ("longitudeSide" in data && data.longitudeSide != undefined) {
                this.longitudeSide = data.longitudeSide;
            }
            if ("country" in data && data.country != undefined) {
                this.country = data.country;
            }
            if ("region" in data && data.region != undefined) {
                this.region = data.region;
            }
            if ("locality" in data && data.locality != undefined) {
                this.locality = data.locality;
            }
            if ("address" in data && data.address != undefined) {
                this.address = data.address;
            }
            if ("venue" in data && data.venue != undefined) {
                this.venue = data.venue;
            }
            if ("thumbnail" in data && data.thumbnail != undefined) {
                this.thumbnail = data.thumbnail;
            }
            if ("isFavorite" in data && data.isFavorite != undefined) {
                this.isFavorite = data.isFavorite;
            }
            if ("albumName" in data && data.albumName != undefined) {
                this.albumName = data.albumName;
            }
        }
    }
    get id() {
        return pb_1.Message.getFieldWithDefault(this, 1, "") as string;
    }
    set id(value: string) {
        pb_1.Message.setField(this, 1, value);
    }
    get thumbnailUrl() {
        return pb_1.Message.getFieldWithDefault(this, 2, "") as string;
    }
    set thumbnailUrl(value: string) {
        pb_1.Message.setField(this, 2, value);
    }
    get mediaUrl() {
        return pb_1.Message.getFieldWithDefault(this, 3, "") as string;
    }
    set mediaUrl(value: string) {
        pb_1.Message.setField(this, 3, value);
    }
    get fullPath() {
        return pb_1.Message.getFieldWithDefault(this, 4, "") as string;
    }
    set fullPath(value: string) {
        pb_1.Message.setField(this, 4, value);
    }
    get fileName() {
        return pb_1.Message.getFieldWithDefault(this, 5, "") as string;
    }
    set fileName(value: string) {
        pb_1.Message.setField(this, 5, value);
    }
    get fileExtension() {
        return pb_1.Message.getFieldWithDefault(this, 6, "") as string;
    }
    set fileExtension(value: string) {
        pb_1.Message.setField(this, 6, value);
    }
    get mediaType() {
        return pb_1.Message.getFieldWithDefault(this, 7, MediaType.IMAGE) as MediaType;
    }
    set mediaType(value: MediaType) {
        pb_1.Message.setField(this, 7, value);
    }
    get fileSizeKb() {
        return pb_1.Message.getFieldWithDefault(this, 8, 0) as number;
    }
    set fileSizeKb(value: number) {
        pb_1.Message.setField(this, 8, value);
    }
    get dateTimeOriginal() {
        return pb_1.Message.getFieldWithDefault(this, 9, 0) as number;
    }
    set dateTimeOriginal(value: number) {
        pb_1.Message.setField(this, 9, value);
    }
    get width() {
        return pb_1.Message.getFieldWithDefault(this, 10, 0) as number;
    }
    set width(value: number) {
        pb_1.Message.setField(this, 10, value);
    }
    get height() {
        return pb_1.Message.getFieldWithDefault(this, 11, 0) as number;
    }
    set height(value: number) {
        pb_1.Message.setField(this, 11, value);
    }
    get thumbnailWidth() {
        return pb_1.Message.getFieldWithDefault(this, 12, 0) as number;
    }
    set thumbnailWidth(value: number) {
        pb_1.Message.setField(this, 12, value);
    }
    get thumbnailHeight() {
        return pb_1.Message.getFieldWithDefault(this, 13, 0) as number;
    }
    set thumbnailHeight(value: number) {
        pb_1.Message.setField(this, 13, value);
    }
    get videoDurationSec() {
        return pb_1.Message.getFieldWithDefault(this, 14, 0) as number;
    }
    set videoDurationSec(value: number) {
        pb_1.Message.setOneofField(this, 14, this.#one_of_decls[0], value);
    }
    get has_videoDurationSec() {
        return pb_1.Message.getField(this, 14) != null;
    }
    get pictureMaker() {
        return pb_1.Message.getFieldWithDefault(this, 15, "") as string;
    }
    set pictureMaker(value: string) {
        pb_1.Message.setOneofField(this, 15, this.#one_of_decls[1], value);
    }
    get has_pictureMaker() {
        return pb_1.Message.getField(this, 15) != null;
    }
    get tag() {
        return pb_1.Message.getFieldWithDefault(this, 16, "") as string;
    }
    set tag(value: string) {
        pb_1.Message.setOneofField(this, 16, this.#one_of_decls[2], value);
    }
    get has_tag() {
        return pb_1.Message.getField(this, 16) != null;
    }
    get latitude() {
        return pb_1.Message.getFieldWithDefault(this, 17, 0) as number;
    }
    set latitude(value: number) {
        pb_1.Message.setOneofField(this, 17, this.#one_of_decls[3], value);
    }
    get has_latitude() {
        return pb_1.Message.getField(this, 17) != null;
    }
    get latitudePole() {
        return pb_1.Message.getFieldWithDefault(this, 18, "") as string;
    }
    set latitudePole(value: string) {
        pb_1.Message.setOneofField(this, 18, this.#one_of_decls[4], value);
    }
    get has_latitudePole() {
        return pb_1.Message.getField(this, 18) != null;
    }
    get longitude() {
        return pb_1.Message.getFieldWithDefault(this, 19, 0) as number;
    }
    set longitude(value: number) {
        pb_1.Message.setOneofField(this, 19, this.#one_of_decls[5], value);
    }
    get has_longitude() {
        return pb_1.Message.getField(this, 19) != null;
    }
    get longitudeSide() {
        return pb_1.Message.getFieldWithDefault(this, 20, "") as string;
    }
    set longitudeSide(value: string) {
        pb_1.Message.setOneofField(this, 20, this.#one_of_decls[6], value);
    }
    get has_longitudeSide() {
        return pb_1.Message.getField(this, 20) != null;
    }
    get country() {
        return pb_1.Message.getFieldWithDefault(this, 21, "") as string;
    }
    set country(value: string) {
        pb_1.Message.setOneofField(this, 21, this.#one_of_decls[7], value);
    }
    get has_country() {
        return pb_1.Message.getField(this, 21) != null;
    }
    get region() {
        return pb_1.Message.getFieldWithDefault(this, 22, "") as string;
    }
    set region(value: string) {
        pb_1.Message.setOneofField(this, 22, this.#one_of_decls[8], value);
    }
    get has_region() {
        return pb_1.Message.getField(this, 22) != null;
    }
    get locality() {
        return pb_1.Message.getFieldWithDefault(this, 23, "") as string;
    }
    set locality(value: string) {
        pb_1.Message.setOneofField(this, 23, this.#one_of_decls[9], value);
    }
    get has_locality() {
        return pb_1.Message.getField(this, 23) != null;
    }
    get address() {
        return pb_1.Message.getFieldWithDefault(this, 24, "") as string;
    }
    set address(value: string) {
        pb_1.Message.setOneofField(this, 24, this.#one_of_decls[10], value);
    }
    get has_address() {
        return pb_1.Message.getField(this, 24) != null;
    }
    get venue() {
        return pb_1.Message.getFieldWithDefault(this, 25, "") as string;
    }
    set venue(value: string) {
        pb_1.Message.setOneofField(this, 25, this.#one_of_decls[11], value);
    }
    get has_venue() {
        return pb_1.Message.getField(this, 25) != null;
    }
    get thumbnail() {
        return pb_1.Message.getFieldWithDefault(this, 26, new Uint8Array()) as Uint8Array;
    }
    set thumbnail(value: Uint8Array) {
        pb_1.Message.setField(this, 26, value);
    }
    get isFavorite() {
        return pb_1.Message.getFieldWithDefault(this, 27, false) as boolean;
    }
    set isFavorite(value: boolean) {
        pb_1.Message.setOneofField(this, 27, this.#one_of_decls[12], value);
    }
    get has_isFavorite() {
        return pb_1.Message.getField(this, 27) != null;
    }
    get albumName() {
        return pb_1.Message.getFieldWithDefault(this, 28, "") as string;
    }
    set albumName(value: string) {
        pb_1.Message.setOneofField(this, 28, this.#one_of_decls[13], value);
    }
    get has_albumName() {
        return pb_1.Message.getField(this, 28) != null;
    }
    get _videoDurationSec() {
        const cases: {
            [index: number]: "none" | "videoDurationSec";
        } = {
            0: "none",
            14: "videoDurationSec"
        };
        return cases[pb_1.Message.computeOneofCase(this, [14])];
    }
    get _pictureMaker() {
        const cases: {
            [index: number]: "none" | "pictureMaker";
        } = {
            0: "none",
            15: "pictureMaker"
        };
        return cases[pb_1.Message.computeOneofCase(this, [15])];
    }
    get _tag() {
        const cases: {
            [index: number]: "none" | "tag";
        } = {
            0: "none",
            16: "tag"
        };
        return cases[pb_1.Message.computeOneofCase(this, [16])];
    }
    get _latitude() {
        const cases: {
            [index: number]: "none" | "latitude";
        } = {
            0: "none",
            17: "latitude"
        };
        return cases[pb_1.Message.computeOneofCase(this, [17])];
    }
    get _latitudePole() {
        const cases: {
            [index: number]: "none" | "latitudePole";
        } = {
            0: "none",
            18: "latitudePole"
        };
        return cases[pb_1.Message.computeOneofCase(this, [18])];
    }
    get _longitude() {
        const cases: {
            [index: number]: "none" | "longitude";
        } = {
            0: "none",
            19: "longitude"
        };
        return cases[pb_1.Message.computeOneofCase(this, [19])];
    }
    get _longitudeSide() {
        const cases: {
            [index: number]: "none" | "longitudeSide";
        } = {
            0: "none",
            20: "longitudeSide"
        };
        return cases[pb_1.Message.computeOneofCase(this, [20])];
    }
    get _country() {
        const cases: {
            [index: number]: "none" | "country";
        } = {
            0: "none",
            21: "country"
        };
        return cases[pb_1.Message.computeOneofCase(this, [21])];
    }
    get _region() {
        const cases: {
            [index: number]: "none" | "region";
        } = {
            0: "none",
            22: "region"
        };
        return cases[pb_1.Message.computeOneofCase(this, [22])];
    }
    get _locality() {
        const cases: {
            [index: number]: "none" | "locality";
        } = {
            0: "none",
            23: "locality"
        };
        return cases[pb_1.Message.computeOneofCase(this, [23])];
    }
    get _address() {
        const cases: {
            [index: number]: "none" | "address";
        } = {
            0: "none",
            24: "address"
        };
        return cases[pb_1.Message.computeOneofCase(this, [24])];
    }
    get _venue() {
        const cases: {
            [index: number]: "none" | "venue";
        } = {
            0: "none",
            25: "venue"
        };
        return cases[pb_1.Message.computeOneofCase(this, [25])];
    }
    get _isFavorite() {
        const cases: {
            [index: number]: "none" | "isFavorite";
        } = {
            0: "none",
            27: "isFavorite"
        };
        return cases[pb_1.Message.computeOneofCase(this, [27])];
    }
    get _albumName() {
        const cases: {
            [index: number]: "none" | "albumName";
        } = {
            0: "none",
            28: "albumName"
        };
        return cases[pb_1.Message.computeOneofCase(this, [28])];
    }
    static fromObject(data: {
        id?: string;
        thumbnailUrl?: string;
        mediaUrl?: string;
        fullPath?: string;
        fileName?: string;
        fileExtension?: string;
        mediaType?: MediaType;
        fileSizeKb?: number;
        dateTimeOriginal?: number;
        width?: number;
        height?: number;
        thumbnailWidth?: number;
        thumbnailHeight?: number;
        videoDurationSec?: number;
        pictureMaker?: string;
        tag?: string;
        latitude?: number;
        latitudePole?: string;
        longitude?: number;
        longitudeSide?: string;
        country?: string;
        region?: string;
        locality?: string;
        address?: string;
        venue?: string;
        thumbnail?: Uint8Array;
        isFavorite?: boolean;
        albumName?: string;
    }): MediaInfo {
        const message = new MediaInfo({});
        if (data.id != null) {
            message.id = data.id;
        }
        if (data.thumbnailUrl != null) {
            message.thumbnailUrl = data.thumbnailUrl;
        }
        if (data.mediaUrl != null) {
            message.mediaUrl = data.mediaUrl;
        }
        if (data.fullPath != null) {
            message.fullPath = data.fullPath;
        }
        if (data.fileName != null) {
            message.fileName = data.fileName;
        }
        if (data.fileExtension != null) {
            message.fileExtension = data.fileExtension;
        }
        if (data.mediaType != null) {
            message.mediaType = data.mediaType;
        }
        if (data.fileSizeKb != null) {
            message.fileSizeKb = data.fileSizeKb;
        }
        if (data.dateTimeOriginal != null) {
            message.dateTimeOriginal = data.dateTimeOriginal;
        }
        if (data.width != null) {
            message.width = data.width;
        }
        if (data.height != null) {
            message.height = data.height;
        }
        if (data.thumbnailWidth != null) {
            message.thumbnailWidth = data.thumbnailWidth;
        }
        if (data.thumbnailHeight != null) {
            message.thumbnailHeight = data.thumbnailHeight;
        }
        if (data.videoDurationSec != null) {
            message.videoDurationSec = data.videoDurationSec;
        }
        if (data.pictureMaker != null) {
            message.pictureMaker = data.pictureMaker;
        }
        if (data.tag != null) {
            message.tag = data.tag;
        }
        if (data.latitude != null) {
            message.latitude = data.latitude;
        }
        if (data.latitudePole != null) {
            message.latitudePole = data.latitudePole;
        }
        if (data.longitude != null) {
            message.longitude = data.longitude;
        }
        if (data.longitudeSide != null) {
            message.longitudeSide = data.longitudeSide;
        }
        if (data.country != null) {
            message.country = data.country;
        }
        if (data.region != null) {
            message.region = data.region;
        }
        if (data.locality != null) {
            message.locality = data.locality;
        }
        if (data.address != null) {
            message.address = data.address;
        }
        if (data.venue != null) {
            message.venue = data.venue;
        }
        if (data.thumbnail != null) {
            message.thumbnail = data.thumbnail;
        }
        if (data.isFavorite != null) {
            message.isFavorite = data.isFavorite;
        }
        if (data.albumName != null) {
            message.albumName = data.albumName;
        }
        return message;
    }
    toObject() {
        const data: {
            id?: string;
            thumbnailUrl?: string;
            mediaUrl?: string;
            fullPath?: string;
            fileName?: string;
            fileExtension?: string;
            mediaType?: MediaType;
            fileSizeKb?: number;
            dateTimeOriginal?: number;
            width?: number;
            height?: number;
            thumbnailWidth?: number;
            thumbnailHeight?: number;
            videoDurationSec?: number;
            pictureMaker?: string;
            tag?: string;
            latitude?: number;
            latitudePole?: string;
            longitude?: number;
            longitudeSide?: string;
            country?: string;
            region?: string;
            locality?: string;
            address?: string;
            venue?: string;
            thumbnail?: Uint8Array;
            isFavorite?: boolean;
            albumName?: string;
        } = {};
        if (this.id != null) {
            data.id = this.id;
        }
        if (this.thumbnailUrl != null) {
            data.thumbnailUrl = this.thumbnailUrl;
        }
        if (this.mediaUrl != null) {
            data.mediaUrl = this.mediaUrl;
        }
        if (this.fullPath != null) {
            data.fullPath = this.fullPath;
        }
        if (this.fileName != null) {
            data.fileName = this.fileName;
        }
        if (this.fileExtension != null) {
            data.fileExtension = this.fileExtension;
        }
        if (this.mediaType != null) {
            data.mediaType = this.mediaType;
        }
        if (this.fileSizeKb != null) {
            data.fileSizeKb = this.fileSizeKb;
        }
        if (this.dateTimeOriginal != null) {
            data.dateTimeOriginal = this.dateTimeOriginal;
        }
        if (this.width != null) {
            data.width = this.width;
        }
        if (this.height != null) {
            data.height = this.height;
        }
        if (this.thumbnailWidth != null) {
            data.thumbnailWidth = this.thumbnailWidth;
        }
        if (this.thumbnailHeight != null) {
            data.thumbnailHeight = this.thumbnailHeight;
        }
        if (this.videoDurationSec != null) {
            data.videoDurationSec = this.videoDurationSec;
        }
        if (this.pictureMaker != null) {
            data.pictureMaker = this.pictureMaker;
        }
        if (this.tag != null) {
            data.tag = this.tag;
        }
        if (this.latitude != null) {
            data.latitude = this.latitude;
        }
        if (this.latitudePole != null) {
            data.latitudePole = this.latitudePole;
        }
        if (this.longitude != null) {
            data.longitude = this.longitude;
        }
        if (this.longitudeSide != null) {
            data.longitudeSide = this.longitudeSide;
        }
        if (this.country != null) {
            data.country = this.country;
        }
        if (this.region != null) {
            data.region = this.region;
        }
        if (this.locality != null) {
            data.locality = this.locality;
        }
        if (this.address != null) {
            data.address = this.address;
        }
        if (this.venue != null) {
            data.venue = this.venue;
        }
        if (this.thumbnail != null) {
            data.thumbnail = this.thumbnail;
        }
        if (this.isFavorite != null) {
            data.isFavorite = this.isFavorite;
        }
        if (this.albumName != null) {
            data.albumName = this.albumName;
        }
        return data;
    }
    serialize(): Uint8Array;
    serialize(w: pb_1.BinaryWriter): void;
    serialize(w?: pb_1.BinaryWriter): Uint8Array | void {
        const writer = w || new pb_1.BinaryWriter();
        if (this.id.length)
            writer.writeString(1, this.id);
        if (this.thumbnailUrl.length)
            writer.writeString(2, this.thumbnailUrl);
        if (this.mediaUrl.length)
            writer.writeString(3, this.mediaUrl);
        if (this.fullPath.length)
            writer.writeString(4, this.fullPath);
        if (this.fileName.length)
            writer.writeString(5, this.fileName);
        if (this.fileExtension.length)
            writer.writeString(6, this.fileExtension);
        if (this.mediaType != MediaType.IMAGE)
            writer.writeEnum(7, this.mediaType);
        if (this.fileSizeKb != 0)
            writer.writeInt64(8, this.fileSizeKb);
        if (this.dateTimeOriginal != 0)
            writer.writeInt64(9, this.dateTimeOriginal);
        if (this.width != 0)
            writer.writeInt32(10, this.width);
        if (this.height != 0)
            writer.writeInt32(11, this.height);
        if (this.thumbnailWidth != 0)
            writer.writeInt32(12, this.thumbnailWidth);
        if (this.thumbnailHeight != 0)
            writer.writeInt32(13, this.thumbnailHeight);
        if (this.has_videoDurationSec)
            writer.writeInt32(14, this.videoDurationSec);
        if (this.has_pictureMaker)
            writer.writeString(15, this.pictureMaker);
        if (this.has_tag)
            writer.writeString(16, this.tag);
        if (this.has_latitude)
            writer.writeDouble(17, this.latitude);
        if (this.has_latitudePole)
            writer.writeString(18, this.latitudePole);
        if (this.has_longitude)
            writer.writeDouble(19, this.longitude);
        if (this.has_longitudeSide)
            writer.writeString(20, this.longitudeSide);
        if (this.has_country)
            writer.writeString(21, this.country);
        if (this.has_region)
            writer.writeString(22, this.region);
        if (this.has_locality)
            writer.writeString(23, this.locality);
        if (this.has_address)
            writer.writeString(24, this.address);
        if (this.has_venue)
            writer.writeString(25, this.venue);
        if (this.thumbnail.length)
            writer.writeBytes(26, this.thumbnail);
        if (this.has_isFavorite)
            writer.writeBool(27, this.isFavorite);
        if (this.has_albumName)
            writer.writeString(28, this.albumName);
        if (!w)
            return writer.getResultBuffer();
    }
    static deserialize(bytes: Uint8Array | pb_1.BinaryReader): MediaInfo {
        const reader = bytes instanceof pb_1.BinaryReader ? bytes : new pb_1.BinaryReader(bytes), message = new MediaInfo();
        while (reader.nextField()) {
            if (reader.isEndGroup())
                break;
            switch (reader.getFieldNumber()) {
                case 1:
                    message.id = reader.readString();
                    break;
                case 2:
                    message.thumbnailUrl = reader.readString();
                    break;
                case 3:
                    message.mediaUrl = reader.readString();
                    break;
                case 4:
                    message.fullPath = reader.readString();
                    break;
                case 5:
                    message.fileName = reader.readString();
                    break;
                case 6:
                    message.fileExtension = reader.readString();
                    break;
                case 7:
                    message.mediaType = reader.readEnum();
                    break;
                case 8:
                    message.fileSizeKb = reader.readInt64();
                    break;
                case 9:
                    message.dateTimeOriginal = reader.readInt64();
                    break;
                case 10:
                    message.width = reader.readInt32();
                    break;
                case 11:
                    message.height = reader.readInt32();
                    break;
                case 12:
                    message.thumbnailWidth = reader.readInt32();
                    break;
                case 13:
                    message.thumbnailHeight = reader.readInt32();
                    break;
                case 14:
                    message.videoDurationSec = reader.readInt32();
                    break;
                case 15:
                    message.pictureMaker = reader.readString();
                    break;
                case 16:
                    message.tag = reader.readString();
                    break;
                case 17:
                    message.latitude = reader.readDouble();
                    break;
                case 18:
                    message.latitudePole = reader.readString();
                    break;
                case 19:
                    message.longitude = reader.readDouble();
                    break;
                case 20:
                    message.longitudeSide = reader.readString();
                    break;
                case 21:
                    message.country = reader.readString();
                    break;
                case 22:
                    message.region = reader.readString();
                    break;
                case 23:
                    message.locality = reader.readString();
                    break;
                case 24:
                    message.address = reader.readString();
                    break;
                case 25:
                    message.venue = reader.readString();
                    break;
                case 26:
                    message.thumbnail = reader.readBytes();
                    break;
                case 27:
                    message.isFavorite = reader.readBool();
                    break;
                case 28:
                    message.albumName = reader.readString();
                    break;
                default: reader.skipField();
            }
        }
        return message;
    }
    serializeBinary(): Uint8Array {
        return this.serialize();
    }
    static deserializeBinary(bytes: Uint8Array): MediaInfo {
        return MediaInfo.deserialize(bytes);
    }
}
export class ImportStepReport extends pb_1.Message {
    #one_of_decls: number[][] = [];
    constructor(data?: any[] | {
        id?: string;
        timestamp?: number;
        severity?: ImportStepReportSeverity;
        stepMessage?: string;
    }) {
        super();
        pb_1.Message.initialize(this, Array.isArray(data) ? data : [], 0, -1, [], this.#one_of_decls);
        if (!Array.isArray(data) && typeof data == "object") {
            if ("id" in data && data.id != undefined) {
                this.id = data.id;
            }
            if ("timestamp" in data && data.timestamp != undefined) {
                this.timestamp = data.timestamp;
            }
            if ("severity" in data && data.severity != undefined) {
                this.severity = data.severity;
            }
            if ("stepMessage" in data && data.stepMessage != undefined) {
                this.stepMessage = data.stepMessage;
            }
        }
    }
    get id() {
        return pb_1.Message.getFieldWithDefault(this, 1, "") as string;
    }
    set id(value: string) {
        pb_1.Message.setField(this, 1, value);
    }
    get timestamp() {
        return pb_1.Message.getFieldWithDefault(this, 2, 0) as number;
    }
    set timestamp(value: number) {
        pb_1.Message.setField(this, 2, value);
    }
    get severity() {
        return pb_1.Message.getFieldWithDefault(this, 3, ImportStepReportSeverity.INFORMATION) as ImportStepReportSeverity;
    }
    set severity(value: ImportStepReportSeverity) {
        pb_1.Message.setField(this, 3, value);
    }
    get stepMessage() {
        return pb_1.Message.getFieldWithDefault(this, 4, "") as string;
    }
    set stepMessage(value: string) {
        pb_1.Message.setField(this, 4, value);
    }
    static fromObject(data: {
        id?: string;
        timestamp?: number;
        severity?: ImportStepReportSeverity;
        stepMessage?: string;
    }): ImportStepReport {
        const message = new ImportStepReport({});
        if (data.id != null) {
            message.id = data.id;
        }
        if (data.timestamp != null) {
            message.timestamp = data.timestamp;
        }
        if (data.severity != null) {
            message.severity = data.severity;
        }
        if (data.stepMessage != null) {
            message.stepMessage = data.stepMessage;
        }
        return message;
    }
    toObject() {
        const data: {
            id?: string;
            timestamp?: number;
            severity?: ImportStepReportSeverity;
            stepMessage?: string;
        } = {};
        if (this.id != null) {
            data.id = this.id;
        }
        if (this.timestamp != null) {
            data.timestamp = this.timestamp;
        }
        if (this.severity != null) {
            data.severity = this.severity;
        }
        if (this.stepMessage != null) {
            data.stepMessage = this.stepMessage;
        }
        return data;
    }
    serialize(): Uint8Array;
    serialize(w: pb_1.BinaryWriter): void;
    serialize(w?: pb_1.BinaryWriter): Uint8Array | void {
        const writer = w || new pb_1.BinaryWriter();
        if (this.id.length)
            writer.writeString(1, this.id);
        if (this.timestamp != 0)
            writer.writeInt64(2, this.timestamp);
        if (this.severity != ImportStepReportSeverity.INFORMATION)
            writer.writeEnum(3, this.severity);
        if (this.stepMessage.length)
            writer.writeString(4, this.stepMessage);
        if (!w)
            return writer.getResultBuffer();
    }
    static deserialize(bytes: Uint8Array | pb_1.BinaryReader): ImportStepReport {
        const reader = bytes instanceof pb_1.BinaryReader ? bytes : new pb_1.BinaryReader(bytes), message = new ImportStepReport();
        while (reader.nextField()) {
            if (reader.isEndGroup())
                break;
            switch (reader.getFieldNumber()) {
                case 1:
                    message.id = reader.readString();
                    break;
                case 2:
                    message.timestamp = reader.readInt64();
                    break;
                case 3:
                    message.severity = reader.readEnum();
                    break;
                case 4:
                    message.stepMessage = reader.readString();
                    break;
                default: reader.skipField();
            }
        }
        return message;
    }
    serializeBinary(): Uint8Array {
        return this.serialize();
    }
    static deserializeBinary(bytes: Uint8Array): ImportStepReport {
        return ImportStepReport.deserialize(bytes);
    }
}
