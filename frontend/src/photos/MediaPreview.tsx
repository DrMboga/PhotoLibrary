import * as React from 'react';
import { useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Tooltip,
  Typography,
} from '@mui/material';
import { dateFromUnixTime } from '../helpers/date-helper';
import FavoriteIcon from '@mui/icons-material/Favorite';
import { MediaInfo, MediaType } from '../model/media-info';
import { blobToImage } from '../helpers/blob-image.helper';
import DownloadIcon from '@mui/icons-material/Download';
import DeleteIcon from '@mui/icons-material/Delete';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import { useAppSelector } from '../storeHooks';
import { selectToken } from '../authentication/authSlice';
import { backendAPI } from '../api/BackendApi';
import CircularProgress from '@mui/material/CircularProgress';
import ScheduleIcon from '@mui/icons-material/Schedule';
import PlaceIcon from '@mui/icons-material/Place';
import SellIcon from '@mui/icons-material/Sell';
import CameraEnhanceIcon from '@mui/icons-material/CameraEnhance';
import { buildAlbumName, checkIsImportant, checkIsPrint } from '../helpers/album-helper';
import StarIcon from '@mui/icons-material/Star';
import StarOutlineIcon from '@mui/icons-material/StarOutline';
import LocalPrintshopIcon from '@mui/icons-material/LocalPrintshop';
import LocalPrintshopOutlinedIcon from '@mui/icons-material/LocalPrintshopOutlined';

type Props = {
  media?: MediaInfo;
  open: boolean;
  onClose: () => void;
  deleteCardFromList: (mediaIdToDelete: string) => void;
  albumMarkChanged: (mediaId: string, album: string) => void;
};
export const MediaPreview = ({
  media,
  open,
  onClose,
  deleteCardFromList,
  albumMarkChanged,
}: Props) => {
  const mediaDate = dateFromUnixTime(media?.dateTimeOriginal ?? 0);

  const address = `${media?.latitude ?? ''}, ${media?.longitude ?? ''} | ${media?.venue ?? ''} | ${
    media?.address ?? ''
  } | ${media?.locality ?? ''} | ${media?.region ?? ''} | ${media?.country ?? ''}`;
  const textMinWidth =
    (((media?.thumbnailWidth ?? 0) < (media?.thumbnailHeight ?? 0)
      ? media?.thumbnailWidth
      : media?.thumbnailHeight) ?? 0) * 1.2;

  const [isFavorite, setIsFavorite] = useState(media?.isFavorite ?? false);
  const [isImportant, setIsImportant] = useState(checkIsImportant(media?.albumName));
  const [isPrint, setIsPrint] = useState(checkIsPrint(media?.albumName));

  const authToken = useAppSelector(selectToken);
  const [mediaData, setMediaData] = useState<Blob | undefined>(undefined);
  const [mediaDataAsUint8, setMediaDataAsUint8] = useState<Uint8Array>(new Uint8Array());
  const [mediaLoading, setMediaLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | undefined>(undefined);

  useEffect(() => {
    if (!media || !open) {
      return;
    }
    setIsFavorite(media?.isFavorite ?? false);
    setIsImportant(checkIsImportant(media?.albumName));
    setIsPrint(checkIsPrint(media?.albumName));

    setMediaLoading(true);
    backendAPI
      .downloadMedia(media.fullPath, authToken)
      .then((value) => {
        setMediaData(value);
        value.arrayBuffer().then((valueAsArray) => {
          setMediaLoading(false);
          setMediaDataAsUint8(new Uint8Array(valueAsArray));
        });
      })
      .catch((err) => setError(err));
  }, [media, open, authToken]);

  const handleFavoriteClick = () => {
    if (!media) {
      return;
    }
    const flagToSet = !isFavorite;
    backendAPI.setMediaAlbumFlag(media.id, flagToSet, undefined, undefined, authToken).then(() => {
      setIsFavorite(flagToSet);
      albumMarkChanged(media.id, buildAlbumName(flagToSet, isImportant, isPrint));
    });
  };

  const handleImportantClick = () => {
    if (!media) {
      return;
    }
    const flagToSet = !isImportant;
    backendAPI.setMediaAlbumFlag(media.id, undefined, flagToSet, undefined, authToken).then(() => {
      setIsImportant(flagToSet);
      albumMarkChanged(media.id, buildAlbumName(isFavorite, flagToSet, isPrint));
    });
  };
  const handlePrintClick = () => {
    if (!media) {
      return;
    }
    const flagToSet = !isPrint;
    backendAPI.setMediaAlbumFlag(media.id, undefined, undefined, flagToSet, authToken).then(() => {
      setIsPrint(flagToSet);
      albumMarkChanged(media.id, buildAlbumName(isFavorite, isImportant, flagToSet));
    });
  };

  const handleDownload = () => {
    if (media && mediaData) {
      const mediaUrl = window.URL.createObjectURL(mediaData);
      const tempLink = document.createElement('a');
      tempLink.href = mediaUrl;
      tempLink.setAttribute('download', media.fileName);
      tempLink.click();
    }
  };

  const handleDelete = () => {
    if (!media?.id) {
      return;
    }
    backendAPI.deleteMedia(media.id, authToken).then(() => {
      deleteCardFromList(media.id);
      handleClose();
    });
  };

  const handleClose = () => {
    setMediaData(undefined);
    setMediaDataAsUint8(new Uint8Array());
    onClose();
  };

  return media ? (
    <Dialog onClose={handleClose} open={open}>
      <Tooltip title={media.fullPath}>
        <DialogTitle>{media.fileName}</DialogTitle>
      </Tooltip>

      {error && <Alert severity="error">{error}</Alert>}
      {mediaLoading && <CircularProgress />}
      {!mediaLoading && media.mediaType === MediaType.IMAGE && (
        <img
          src={blobToImage(mediaDataAsUint8)}
          alt={media.fileName}
          style={{ height: media.thumbnailHeight * 2.2 }}
        />
      )}
      {!mediaLoading && media.mediaType === MediaType.VIDEO && mediaData && (
        <video
          controls
          style={{ height: media.thumbnailHeight === 0 ? 504 : media.thumbnailHeight * 3 }}
          playsInline
        >
          <source src={URL.createObjectURL(mediaData)} />
        </video>
      )}
      <DialogContent sx={{ paddingTop: '1px', paddingBottom: '0px' }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: '3px' }}>
          <ScheduleIcon fontSize="small" />
          <Typography id={`typography-1-${media.id}`} variant="body2">
            {mediaDate.toLocaleString('ru-RU')}
          </Typography>
        </Box>
        {media.tag && (
          <Box sx={{ display: 'flex', alignItems: 'center', gap: '3px', marginTop: '2px' }}>
            <SellIcon fontSize="small" />
            <Typography id={`typography-2-${media.id}`} variant="body2">
              {media.tag}
            </Typography>
          </Box>
        )}
        {media.pictureMaker && (
          <Box sx={{ display: 'flex', alignItems: 'center', gap: '3px', marginTop: '2px' }}>
            <CameraEnhanceIcon fontSize="small" />
            <Typography
              id={`typography-4-${media.id}`}
              sx={{
                fontSize: '10px',
                display: 'flex',
                alignItems: 'center',
              }}
            >
              {media.pictureMaker}
            </Typography>
          </Box>
        )}
        {media.country && (
          <Tooltip title={address}>
            <Box
              sx={{
                display: 'flex',
                alignItems: 'center',
                gap: '3px',
                marginTop: '2px',
              }}
            >
              <PlaceIcon fontSize="small" />
              <Typography
                id={`typography-3-${media.id}`}
                sx={{
                  fontSize: '12px',
                  display: 'flex',
                  alignItems: 'center',
                  inlineSize: `${textMinWidth}px`,
                  overflowWrap: 'break-word',
                }}
                color="text.secondary"
              >
                {media.address}
              </Typography>
            </Box>
          </Tooltip>
        )}
      </DialogContent>
      <DialogActions
        disableSpacing
        sx={{ paddingTop: '1px', paddingBottom: '1px', display: 'flex' }}
      >
        <Box sx={{ flexGrow: 1 }}>
          <IconButton aria-label="Add to favorites" size="small" onClick={handleFavoriteClick}>
            {isFavorite ? <FavoriteIcon /> : <FavoriteBorderIcon />}
          </IconButton>
          <IconButton aria-label="Mark as important" size="small" onClick={handleImportantClick}>
            {isImportant ? <StarIcon /> : <StarOutlineIcon />}
          </IconButton>
          <IconButton aria-label="Mark as print" size="small" onClick={handlePrintClick}>
            {isPrint ? <LocalPrintshopIcon /> : <LocalPrintshopOutlinedIcon />}
          </IconButton>
          <IconButton aria-label="Download" size="small" onClick={handleDownload}>
            <DownloadIcon />
          </IconButton>
        </Box>
        <Box sx={{ flexGrow: 0 }}>
          <IconButton aria-label="Delete" size="small" onClick={handleDelete}>
            <DeleteIcon />
          </IconButton>
        </Box>
      </DialogActions>
    </Dialog>
  ) : (
    <Dialog onClose={handleClose} open={open}>
      <Alert severity="error">Media is empty</Alert>
    </Dialog>
  );
};
