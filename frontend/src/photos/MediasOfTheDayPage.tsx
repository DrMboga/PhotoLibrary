// @flow
import * as React from 'react';
import { useEffect, useState } from 'react';
import { MediaInfo } from '../model/media-info';
import {
  currentDateLinuxTime,
  dateFromUnixTime,
  dateToDateInputFormat,
  dateToUnixTime,
  parseDateInput,
} from '../helpers/date-helper';
import { useAppSelector } from '../storeHooks';
import { selectToken } from '../authentication/authSlice';
import { backendAPI } from '../api/BackendApi';
import { Alert, Box, Divider, TextField, Typography } from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import { MediaCard } from './MediaCard';
import { MediaPreview } from './MediaPreview';

const dateInput = 'date-input';

interface PhotosWithLabel {
  label: string;
  photos: MediaInfo[];
}
export function MediasOfTheDayPage() {
  const [today, setToday] = useState(currentDateLinuxTime());
  const [todayAsString, setTodayAsString] = useState<string>(dateToDateInputFormat(new Date()));
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | undefined>(undefined);
  const [photos, setPhotos] = useState<PhotosWithLabel[]>([]);
  const [selectedMedia, setSelectedMedia] = useState<MediaInfo>();
  const [dialogOpen, setDialogOpen] = useState(false);

  const authToken = useAppSelector(selectToken);

  useEffect(() => {
    setIsLoading(true);
    backendAPI
      .getMediasOfTheDay(today, authToken)
      .then((media) => {
        setIsLoading(false);
        setError(undefined);
        const sortedPhotos = sortPhotos(media, today);
        setPhotos(sortedPhotos);
      })
      .catch((err) => {
        setIsLoading(false);
        setPhotos([]);
        setError(err.message);
      });
  }, [today, authToken]);

  const sortPhotos = (photos: MediaInfo[], todaySelected: number): PhotosWithLabel[] => {
    const todaySelectedAsDate = dateFromUnixTime(todaySelected);
    const result: PhotosWithLabel[] = [];
    const allYears: number[] = [
      ...new Set(
        photos.map((p) => {
          const currentDate = dateFromUnixTime(p.dateTimeOriginal);
          return currentDate.getFullYear();
        }),
      ),
    ].sort();

    for (const currentYear of allYears) {
      const options: Intl.DateTimeFormatOptions = {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric',
      };
      const label = new Date(
        currentYear,
        todaySelectedAsDate.getMonth(),
        todaySelectedAsDate.getDate(),
      ).toLocaleDateString('en-US', options);
      const medias = photos.filter((p) => {
        const currentDate = dateFromUnixTime(p.dateTimeOriginal);
        if (currentDate.getFullYear() === currentYear) {
          return p;
        }
      });
      result.push({ label, photos: medias });
    }

    return result;
  };

  const handleInputChange = (event: any) => {
    const { name, value } = event.target;
    if (name === dateInput) {
      setTodayAsString(value);
      const newDate = parseDateInput(value);
      const newDateAsNumber = dateToUnixTime(newDate);
      if (newDateAsNumber !== today) {
        setToday(newDateAsNumber);
      }
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
    const photosCopy = [...photos];
    for (const photosWithLabel of photosCopy) {
      photosWithLabel.photos = photosWithLabel.photos.filter((m) => m.id !== mediaIdToDelete);
    }

    setPhotos(photosCopy);
    handleDialogClose();
  };

  const handleAlbumMarkChanged = (mediaId: string, album: string) => {
    const photosCopy = [...photos];
    const changedPhoto = photosCopy.flatMap((s) => s.photos).find((p) => p.id === mediaId);
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
          sx={{ marginTop: '10px', marginBottom: '10px', display: 'flex', alignItems: 'center' }}
        >
          <TextField
            hiddenLabel
            id="today-input"
            type="date"
            name={dateInput}
            value={todayAsString}
            onChange={handleInputChange}
            variant="filled"
            size="small"
          />
        </Box>
        <Box
          sx={{
            flexGrow: 1,
            display: 'flex',
            flexDirection: 'column',
          }}
        >
          {isLoading && <CircularProgress />}
          {error && <Alert severity="error">{error}</Alert>}
          {!isLoading &&
            !error &&
            photos.length > 0 &&
            photos.map((photoWithLabel) => (
              <Box key={`area-${photoWithLabel.label}`}>
                <Divider />
                <Typography variant="subtitle1" key={`label-${photoWithLabel.label}`}>
                  {photoWithLabel.label}
                </Typography>
                <Box
                  sx={{
                    display: 'flex',
                    flexWrap: 'wrap',
                    gap: '10px',
                    rowGap: '10px',
                    justifyContent: 'space-evenly',
                    alignItems: 'center',
                  }}
                  key={`box-${photoWithLabel.label}`}
                >
                  {photoWithLabel.photos.map((photo) => (
                    <MediaCard
                      key={`media-card-id-${photo.id}`}
                      media={photo}
                      onClick={handleCardClick}
                    />
                  ))}
                </Box>
              </Box>
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
