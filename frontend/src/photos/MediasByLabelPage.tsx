// @flow
import * as React from 'react';
import { useEffect, useState } from 'react';
import {
  currentDateLinuxTime,
  dateFromUnixTime,
  dateToDateInputFormat,
  dateToUnixTime,
  parseDateInput,
} from '../helpers/date-helper';
import { MediaInfo } from '../model/media-info';
import { useAppSelector } from '../storeHooks';
import { selectToken } from '../authentication/authSlice';
import { backendAPI } from '../api/BackendApi';
import { Alert, Box, Divider, InputLabel, Select, TextField, Typography } from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import { MediaCard } from './MediaCard';
import { MediaPreview } from './MediaPreview';
import MenuItem from '@mui/material/MenuItem';

const fromInput = 'from-input';
const toInput = 'to-input';
const labelSelect = 'label-select';

export function MediasByLabelPage() {
  const [dateFrom, setDateFrom] = useState(currentDateLinuxTime() - 3000000);
  const [dateFromAsString, setDateFromAsString] = useState<string>(
    dateToDateInputFormat(dateFromUnixTime(currentDateLinuxTime() - 3000000)),
  );
  const [dateTo, setDateTo] = useState(currentDateLinuxTime());
  const [dateToAsString, setDateToAsString] = useState<string>(dateToDateInputFormat(new Date()));
  const [selectedLabel, setSelectedLabel] = useState<string>('People');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | undefined>(undefined);
  const [photos, setPhotos] = useState<MediaInfo[]>([]);
  const [selectedMedia, setSelectedMedia] = useState<MediaInfo>();
  const [dialogOpen, setDialogOpen] = useState(false);

  const authToken = useAppSelector(selectToken);

  useEffect(() => {
    setIsLoading(true);
    backendAPI
      .getMediasByLabel(dateFrom, dateTo, selectedLabel, authToken)
      .then((media) => {
        setIsLoading(false);
        setError(undefined);
        setPhotos(media);
      })
      .catch((err) => {
        setIsLoading(false);
        setPhotos([]);
        setError(err.message);
      });
  }, [dateFrom, dateTo, selectedLabel, authToken]);

  const handleInputChange = (event: any) => {
    const { name, value } = event.target;
    if (name === fromInput || name === toInput) {
      const newDate = parseDateInput(value);
      const newDateAsNumber = dateToUnixTime(newDate);
      switch (name) {
        case fromInput:
          setDateFromAsString(value);
          if (newDateAsNumber !== dateFrom) {
            setDateFrom(newDateAsNumber);
          }
          break;
        case toInput:
          setDateToAsString(value);
          if (newDateAsNumber !== dateTo) {
            setDateTo(newDateAsNumber);
          }
          break;
      }
    }
    if (name === labelSelect) {
      setSelectedLabel(value);
    }
  };

  const handleCardClick = (media: MediaInfo) => {
    setSelectedMedia(media);
    setDialogOpen(true);
  };

  const handleDialogClose = () => {
    setDialogOpen(false);
    setSelectedMedia(undefined);
  };

  const handleDeleteCard = (mediaIdToDelete: string) => {
    const photosCopy = [...photos.filter((m) => m.id !== mediaIdToDelete)];
    setPhotos(photosCopy);
    handleDialogClose();
  };

  const handleAlbumMarkChanged = (mediaId: string, album: string) => {
    const photosCopy = [...photos];
    const changedPhoto = photosCopy.find((p) => p.id === mediaId);
    if (changedPhoto) {
      changedPhoto.albumName = album;
      changedPhoto.isFavorite = album.includes('Fav');
    }
    setPhotos(photosCopy);
  };

  return (
    <>
      <Box sx={{ display: 'flex', flexDirection: 'column' }}>
        <Box
          sx={{
            marginTop: '10px',
            marginBottom: '10px',
            display: 'flex',
            alignItems: 'center',
            gap: '10px',
          }}
        >
          <TextField
            label="Date from"
            id="date-from-input"
            type="date"
            name={fromInput}
            value={dateFromAsString}
            onChange={handleInputChange}
            variant="filled"
            size="small"
          />
          <TextField
            label="Date to"
            id="date-to-input"
            type="date"
            name={toInput}
            value={dateToAsString}
            onChange={handleInputChange}
            variant="filled"
            size="small"
          />
          <Select
            name={labelSelect}
            id="label-select"
            value={selectedLabel}
            label="Label"
            onChange={handleInputChange}
          >
            <MenuItem value={'People'}>People</MenuItem>
            <MenuItem value={'Document'}>Document</MenuItem>
            <MenuItem value={'Other'}>Other</MenuItem>
          </Select>
        </Box>
        <Box
          sx={{
            flexGrow: 1,
            display: 'flex',
            flexWrap: 'wrap',
            gap: '10px',
            rowGap: '10px',
            justifyContent: 'space-evenly',
            alignItems: 'center',
          }}
        >
          {isLoading && <CircularProgress />}
          {error && <Alert severity="error">{error}</Alert>}
          {!isLoading &&
            !error &&
            photos.length > 0 &&
            photos.map((photo) => (
              <MediaCard
                key={`media-card-id-${photo.id}`}
                media={photo}
                onClick={handleCardClick}
              />
            ))}
        </Box>
      </Box>
      <MediaPreview
        media={selectedMedia}
        open={dialogOpen}
        onClose={handleDialogClose}
        deleteCardFromList={handleDeleteCard}
        albumMarkChanged={handleAlbumMarkChanged}
      />
    </>
  );
}
